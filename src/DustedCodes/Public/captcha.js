var captchaOnload = function () {
    const siteKey = document.getElementById('captchaSiteKey').value;
    const isDarkMode = window.matchMedia("(prefers-color-scheme: dark)").matches
    const theme = isDarkMode ? 'dark' : 'light';
    grecaptcha.render('captcha', {
        'sitekey': siteKey,
        'theme': theme
    });
};