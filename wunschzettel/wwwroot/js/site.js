// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
	// Remove gift confirmation
	var removeButtons = document.querySelectorAll('.remove-gift-form .remove-btn');
	var modalEl = document.getElementById('confirmDeleteModal');
	var bsModal = modalEl ? new bootstrap.Modal(modalEl) : null;
	var formToSubmit = null;

	removeButtons.forEach(function (btn) {
		btn.addEventListener('click', function (e) {
			var form = e.target.closest('form');
			if (!form) return;
			formToSubmit = form;
			var name = form.dataset && form.dataset.giftName ? form.dataset.giftName : '';
			var body = document.getElementById('confirmDeleteModalBody');
			if (body && name) body.textContent = 'Wirklich löschen: "' + name + '" ?';
			if (bsModal) bsModal.show();
		});
	});

	var confirmBtn = document.getElementById('confirmDelete');
	if (confirmBtn) {
		confirmBtn.addEventListener('click', function () {
			if (formToSubmit) formToSubmit.submit();
		});
	}

	// Mark bought modal handling
	var markButtons = document.querySelectorAll('.mark-bought-btn');
	var markModalEl = document.getElementById('markBoughtModal');
	var bsMarkModal = markModalEl ? new bootstrap.Modal(markModalEl) : null;
	markButtons.forEach(function (btn) {
		btn.addEventListener('click', function (e) {
			var id = btn.getAttribute('data-gift-id');
			var name = btn.getAttribute('data-gift-name') || '';
			var input = document.getElementById('markGiftId');
			var buyer = document.getElementById('buyerName');
			if (input) input.value = id || '';
			if (buyer) buyer.value = '';
			if (bsMarkModal) bsMarkModal.show();
		});
	});

	// If the server signalled validation error, open the modal and show the message
	if (markModalEl) {
		var openId = markModalEl.dataset.openId || '';
		var err = markModalEl.dataset.error || '';
		if (openId) {
			var input = document.getElementById('markGiftId');
			var buyer = document.getElementById('buyerName');
			var errEl = document.getElementById('markBoughtError');
			if (input) input.value = openId;
			if (errEl && err) errEl.textContent = err;
			if (bsMarkModal) bsMarkModal.show();
		}
	}
});
