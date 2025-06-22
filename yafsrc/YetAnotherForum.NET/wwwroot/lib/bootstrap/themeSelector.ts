/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2024 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */

(() => {
    'use strict';

    const getCookie = (cookieName: string): string => {
	    const name = `${cookieName}=`;
	    const ca = document.cookie.split(';');
	    for (let i = 0; i < ca.length; i++) {
		    let c: string = ca[i];
		    while (c.charAt(0) === ' ') {
			    c = c.substring(1);
		    }
		    if (c.indexOf(name) === 0) {
			    return c.substring(name.length, c.length);
		    }
	    }
	    return '';
    };

    const getStoredTheme = (): string => getCookie('YAF-Theme');

    const setStoredTheme = (theme: string): void => {
	    const d = new Date();
	    d.setTime(d.getTime() + (365 * 24 * 60 * 60 * 1000));
	    const expires = `expires=${d.toUTCString()}`;

	    document.cookie = `${encodeURIComponent('YAF-Theme')}=${encodeURIComponent(theme)}; ${expires}; path=/`;
    };

    const getPreferredTheme = (): string => {
	    const storedTheme = getStoredTheme();
	    if (storedTheme) {
		    return storedTheme;
	    }

	    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    };

    const setTheme = (theme: string) => {
	    if (theme === 'auto') {
		    document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));
	    } else {
		    document.documentElement.setAttribute('data-bs-theme', theme);
	    }
    };

    setTheme(getPreferredTheme());

	const showActiveTheme = (theme: string): void => {
		const themeSwitcher = document.getElementById('bd-theme') as HTMLButtonElement;

	    if (!themeSwitcher) {
		    return;
	    }

	    const themeSwitcherText = document.getElementById('bd-theme-text') as HTMLElement,
		    activeThemeIcon = document.querySelector('.theme-icon-active') as HTMLElement,
		    btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`) as HTMLElement,
		    iconOfActiveBtn = btnToActive.querySelector('i')?.className || '',
		    checkOfActiveBtn = btnToActive.querySelector('.fa-check') as HTMLElement;

		document.querySelectorAll<HTMLButtonElement>('[data-bs-theme-value]').forEach((element) => {
		    element.classList.remove('active');
		    element.setAttribute('aria-pressed', 'false');
		    const checkIcon = element.querySelector('.fa-check') as HTMLElement;
		    checkIcon.classList.add('d-none');
	    });

	    btnToActive.classList.add('active');
	    btnToActive.setAttribute('aria-pressed', 'true');

	    checkOfActiveBtn.classList.remove('d-none');

	    activeThemeIcon.setAttribute('class', `${iconOfActiveBtn} theme-icon-active`);
	    const themeSwitcherLabel = `${themeSwitcherText.textContent} (${btnToActive.dataset.bsThemeValue})`;
	    themeSwitcher.setAttribute('aria-label', themeSwitcherLabel);
	};

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        const storedTheme = getStoredTheme();
        if (storedTheme !== 'light' && storedTheme !== 'dark') {
            setTheme(getPreferredTheme());
        }
    });

    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme());

        document.querySelectorAll<HTMLButtonElement>('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')! as string;
                    setStoredTheme(theme);
                    setTheme(theme);
                    showActiveTheme(theme);
                });
            });
    });
})()
