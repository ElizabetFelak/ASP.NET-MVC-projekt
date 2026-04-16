---
name: "Lab 2 UI via UX Sub-Agent"
description: "Use for Lab 2 ASP.NET MVC UI work. Makes the main agent delegate UI/UX design to UX UI Specialist, then implement Razor/CSS/navigation changes from that plan."
agent: "agent"
argument-hint: "Describe the entity/page scope (for example: all Index/Details pages and a custom homepage)"
---
Task: Build or update Lab 2 UI in this workspace.

Input scope from user: ${input}

Required workflow:
1. Start by invoking the sub-agent named "UX UI Specialist".
2. Ask that sub-agent for a concrete UI implementation plan and file-level change list for the requested scope.
3. Implement the returned plan directly in this repository (Razor views, layout/navigation, CSS, and minimal controller/view-model wiring if needed for UI rendering).
4. Preserve MVC conventions and route-safe links.
5. Keep UX non-standard and clearly different from default Bootstrap template.

Lab 2 acceptance constraints:
- Implement Index/list pages for each requested entity.
- Implement Details pages for each requested entity.
- Implement one custom page (for example, a distinctive home page concept).
- Ensure full navigation: menu, list-to-details links, breadcrumbs.
- Do not add Create/Edit flows unless explicitly requested.

Output requirements:
1. Summarize what was delegated to the UX sub-agent.
2. List edited files.
3. Explain how the result satisfies Lab 2 requirements.
4. Note any missing data or blockers.
