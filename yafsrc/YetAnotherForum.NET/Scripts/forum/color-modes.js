/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2025 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */

(() => {
    'use strict';

    const getCookie = cookieName => {
	    const name = cookieName + '=';
	    const ca = document.cookie.split(';');
	    for (let i = 0; i < ca.length; i++) {
		    let c = ca[i];
		    while (c.charAt(0) === ' ') {
			    c = c.substring(1);
		    }
		    if (c.indexOf(name) === 0) {
			    return c.substring(name.length, c.length);
		    }
	    }
	    return '';
    }

    const getStoredTheme = () => getCookie('YAF-Theme');
    const setStoredTheme = theme => {
	    const d = new Date();
	    d.setTime(d.getTime() + (365 * 24 * 60 * 60 * 1000));
	    const expires = `expires=${d.toUTCString()}`;

	    document.cookie = `${encodeURIComponent('YAF-Theme')}=${encodeURIComponent(theme)}; ${expires}; path=/`;
    }

    const getPreferredTheme = () => {
        const storedTheme = getStoredTheme();
        if (storedTheme) {
            return storedTheme;
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    };

    const setTheme = theme => {
        if (theme === 'auto') {
            document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));
        } else {
            document.documentElement.setAttribute('data-bs-theme', theme);
        }
    };

    setTheme(getPreferredTheme());

    const showActiveTheme = (theme) => {
        const themeSwitcher = document.getElementById('bd-theme');

        if (!themeSwitcher) {
            return;
        }

        const themeSwitcherText = document.getElementById('bd-theme-text'),
	        activeThemeIcon = document.querySelector('.theme-icon-active'),
	        btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`),
	        iconOfActiveBtn = btnToActive.querySelector('i').className,
            checkOfActiveBtn = btnToActive.querySelector('.fa-check');

        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active');
            element.setAttribute('aria-pressed', 'false');
            element.querySelector('.fa-check').classList.add('d-none');
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

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value');
                    setStoredTheme(theme);
                    setTheme(theme);
                    showActiveTheme(theme, true);
                });
            });
    });
})()
