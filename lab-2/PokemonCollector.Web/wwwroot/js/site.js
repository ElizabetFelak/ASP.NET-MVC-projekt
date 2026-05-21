// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.initAjaxSearch = function (options) {
	var input = document.querySelector(options.inputSelector);
	var grid = document.querySelector(options.gridSelector);

	if (!input || !grid) return;

	var timer = null;
	input.addEventListener('input', function () {
		clearTimeout(timer);
		timer = setTimeout(function () {
			var q = encodeURIComponent(input.value || '');
			fetch(options.searchUrl + '?q=' + q)
				.then(function (r) { return r.json(); })
				.then(function (items) {
					grid.innerHTML = '';
					if (!items || items.length === 0) {
						grid.innerHTML = '<div class="text-muted">No results found.</div>';
						return;
					}
					items.forEach(function (it) {
						grid.insertAdjacentHTML('beforeend', options.renderItem(it));
					});
				});
		}, options.delay || 300);
	});
};
