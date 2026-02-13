function showTab(tab) {
    const isLogin = tab === 'login';
    document.getElementById('panel-login').classList.toggle('hidden', !isLogin);
    document.getElementById('panel-register').classList.toggle('hidden', isLogin);

    const active   = document.getElementById('tab-' + tab);
    const inactive = document.getElementById('tab-' + (isLogin ? 'register' : 'login'));

    active.style.borderBottomColor = '#041f41';
    active.style.color = '#041f41';
    inactive.style.borderBottomColor = 'transparent';
    inactive.style.color = '#9ca3af';
}