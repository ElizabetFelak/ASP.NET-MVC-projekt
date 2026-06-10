using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCollector.Web.Controllers.Api
{
    [Route("api/attachments")]
    [ApiController]
    public class AttachmentsApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public AttachmentsApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/attachments
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<AttachmentDTO>>> GetAllAttachments([FromQuery] int cardsetId = 0)
        {
            var query = _context.Attachments.AsQueryable();

            if (cardsetId > 0)
            {
                query = query.Where(a => a.CardSetId == cardsetId);
            }

            var attachments = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => ToDTO(a))
                .ToListAsync();

            return Ok(attachments);
        }

        // GET: api/cardsets/{cardsetId}/attachments
        [HttpGet("cardset/{cardsetId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AttachmentDTO>>> GetCardSetAttachments(int cardsetId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.CardSetId == cardsetId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => ToDTO(a))
                .ToListAsync();

            return Ok(attachments);
        }

        // GET: api/cards/{cardId}/attachments
        [HttpGet("card/{cardId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AttachmentDTO>>> GetPokemonCardAttachments(int cardId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.PokemonCardId == cardId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => ToDTO(a))
                .ToListAsync();

            return Ok(attachments);
        }

        // POST: api/cardsets/{cardsetId}/attachments/upload
        [Authorize]
        [HttpPost("cardset/{cardsetId}/upload")]
        public async Task<ActionResult<AttachmentDTO>> UploadAttachment(int cardsetId, IFormFile file)
        {
            var cardSet = await _context.CardSets.FindAsync(cardsetId);
            if (cardSet == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            var uploadsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "cardsets",
                cardsetId.ToString());

            Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                CardSetId = cardsetId,
                FileName = file.FileName,
                FilePath = "/uploads/cardsets/" + cardsetId + "/" + fileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCardSetAttachments), new { cardsetId = cardsetId }, ToDTO(attachment));
        }

        // POST: api/cards/{cardId}/attachments/upload
        [Authorize]
        [HttpPost("card/{cardId}/upload")]
        public async Task<ActionResult<AttachmentDTO>> UploadPokemonCardAttachment(int cardId, IFormFile file)
        {
            var card = await _context.PokemonCards.FindAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            var uploadsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "cards",
                cardId.ToString());

            Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                PokemonCardId = cardId,
                FileName = file.FileName,
                FilePath = "/uploads/cards/" + cardId + "/" + fileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPokemonCardAttachments), new { cardId = cardId }, ToDTO(attachment));
        }

        // DELETE: api/attachments/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                attachment.FilePath.TrimStart('/'));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static AttachmentDTO ToDTO(Attachment attachment) =>
            new AttachmentDTO
            {
                Id = attachment.Id,
                CardSetId = attachment.CardSetId,
                PokemonCardId = attachment.PokemonCardId,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                ContentType = attachment.ContentType,
                FileSize = attachment.FileSize,
                CreatedAt = attachment.CreatedAt
            };
    }
}
