// JS Dependencies: Popper, Bootstrap
import '@popperjs/core';
import * as bootstrap from 'bootstrap';
import * as Notifications from './forum/notifications';
import './prism.js';
import Notify from '@w8tcha/bootstrap-notify';

import * as bootbox from '@w8tcha/bootbox';
import './forum/bootstrap-touchspin';
import './forum/hoverCard';
import 'long-press-event';
import './forum/paging';
import './forum/modals';
import './forum/notificationHub';
import './forum/contextMenu';

const _global = (window /* browser */ || global /* node */) as any;

_global.bootstrap = bootstrap;
_global.Notify = Notify;
_global.bootbox = bootbox;

// Generic Functions
document.addEventListener('DOMContentLoaded', () => {
	// handle scroll position on form submit
	document.querySelectorAll<HTMLButtonElement>('button[type="submit"]').forEach(button => {
		if (button.closest('form')) {
			button.addEventListener('click', handleSubmit);
		};
	});

	function handleSubmit() {
		sessionStorage.setItem('scrollPosition', window.scrollY.toString());
	}

	const scrollPosition = sessionStorage.getItem('scrollPosition');

	if (scrollPosition) {
		const pos = parseInt(scrollPosition);

		if (pos > 0) {
			sessionStorage.removeItem('scrollPosition');
			window.scrollTo({ top: pos, behavior: 'instant' });
		}
	}

	const loginButton = document.querySelector<HTMLElement>('a.btn-login,input.btn-login, .btn-spinner')!;
    if (loginButton) {
        loginButton.addEventListener('click', (event) => {
            const button = event.target as HTMLElement;
            button.innerHTML = "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
            button.classList.add('disabled');
        });
    }

    document.querySelectorAll<HTMLElement>('.dropdown-menu a.dropdown-toggle').forEach(menu => {
	    menu.addEventListener('click', (event: Event) => {
		    const $el = menu as HTMLElement;
		    const $subMenu = $el.nextElementSibling as HTMLElement;

		    document.querySelectorAll<HTMLElement>('.dropdown-menu .show').forEach(dropDownMenu => {
			    dropDownMenu.classList.remove('show');
		    });

		    if ($subMenu) {
			    $subMenu.classList.add('show');

			    $subMenu.style.top = `${$el.offsetTop - 10}px`;
			    $subMenu.style.left = `${$el.offsetWidth - 4}px`;
		    }

		    event.stopPropagation();
	    });
    });

    // Notify dropdown
    const dropdownNotify = document.querySelector('.dropdown-notify');
    if (dropdownNotify) {
        dropdownNotify.addEventListener('show.bs.dropdown', () => {
            const pageSize = 5;
            const pageNumber = 0;
            Notifications.getNotifyData(pageSize, pageNumber, false);
        });
    }

    document.querySelectorAll<HTMLElement>('[data-bs-toggle="tooltip"]').forEach(toolTip => {
	    return new bootstrap.Tooltip(toolTip);
    });

    document.querySelectorAll<HTMLElement>('.attachedImage').forEach(imageLink => {
	    var parentNode = (imageLink.parentNode as HTMLElement);
        const messageId = parentNode?.id as string;

        imageLink.setAttribute('data-gallery', `gallery-${messageId}`);
    });

    const scrollToTopBtn = document.querySelector<HTMLButtonElement>('.btn-scroll');
    const rootElement = document.documentElement;

    if (scrollToTopBtn) {
	    function handleScroll(): void {
		    const scrollTotal = rootElement.scrollHeight - rootElement.clientHeight;
		    if ((rootElement.scrollTop / scrollTotal) > 0.15) {
			    // Show button
			    scrollToTopBtn!.classList.add('show-btn-scroll');
		    } else {
			    // Hide button
			    scrollToTopBtn!.classList.remove('show-btn-scroll');
		    }
	    }

	    function scrollToTop(e: Event): void {
		    e.preventDefault();

		    // Scroll to top logic
		    rootElement.scrollTo({
			    top: 0,
			    behavior: 'smooth'
		    });
	    }

	    scrollToTopBtn.addEventListener('click', scrollToTop);
	    document.addEventListener('scroll', handleScroll);
    }
});
