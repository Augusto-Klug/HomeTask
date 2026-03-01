const { createApp } = Vue;

const CATEGORIAS = [
    { id: 1,  nome: 'Faxina',           icone: 'bi-house-heart' },
    { id: 2,  nome: 'Jardinagem',        icone: 'bi-flower1' },
    { id: 3,  nome: 'Reparos',           icone: 'bi-tools' },
    { id: 4,  nome: 'Lavanderia',        icone: 'bi-water' },
    { id: 5,  nome: 'Passadoria',        icone: 'bi-wind' },
    { id: 6,  nome: 'Babysitter',        icone: 'bi-emoji-smile' },
    { id: 7,  nome: 'Cuidador de Idosos',icone: 'bi-heart-pulse' },
    { id: 8,  nome: 'Pet Sitter',        icone: 'bi-emoji-heart-eyes' },
    { id: 9,  nome: 'Cozinheiro',        icone: 'bi-egg-fried' },
    { id: 10, nome: 'Serviços Gerais',   icone: 'bi-gear' },
];

// Navbar dinâmica
const navbar = document.getElementById('navbar-app');
const user = getUser();
navbar.innerHTML = `
<div class="container">
    <a class="navbar-brand" href="/index.html"><span>HomeTask</span></a>
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navMenu">
        <ul class="navbar-nav ms-auto align-items-center gap-2">
            <li class="nav-item"><a class="nav-link" href="/servicos/buscar.html">Buscar Serviços</a></li>
            ${user
                ? `<li class="nav-item"><span class="nav-link fw-semibold">Olá, ${user.nome}</span></li>
                   <li class="nav-item"><a class="btn btn-outline-danger btn-sm" href="#" id="btn-sair">Sair</a></li>`
                : `<li class="nav-item"><a class="btn btn-outline-primary btn-sm" href="/login.html">Entrar</a></li>
                   <li class="nav-item"><a class="btn btn-primary btn-sm" href="/cadastro.html">Cadastrar</a></li>`
            }
        </ul>
    </div>
</div>`;

document.getElementById('btn-sair')?.addEventListener('click', e => {
    e.preventDefault();
    clearSession();
    window.location.reload();
});

if (user) {
    document.getElementById('btn-cadastro')?.setAttribute('style', 'display:none');
}

// Renderiza categorias
const grid = document.getElementById('categorias');
CATEGORIAS.forEach(cat => {
    const col = document.createElement('div');
    col.className = 'col-6 col-sm-4 col-md-3 col-lg-2';
    col.innerHTML = `
        <a href="/servicos/buscar.html?categoria=${cat.id}" class="text-decoration-none">
            <div class="card text-center p-3 h-100 servico-card">
                <i class="bi ${cat.icone} fs-2 text-primary mb-2"></i>
                <small class="fw-semibold text-dark">${cat.nome}</small>
            </div>
        </a>`;
    grid.appendChild(col);
});
