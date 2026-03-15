import ui from "beercss";

export default {
  fecharSidenav(idElemento: string) {
    const elemento = document.getElementById(idElemento);
    console.log(elemento);
    if (elemento?.classList.contains("active")) {
      ui(`#${idElemento}`);
    }
  },
  abiriSidenav(idElemento: string) {
    const elemento = document.getElementById(idElemento);
    if (!elemento?.classList.contains("active")) {
      ui(`#${idElemento}`);
    }
  },
  fecharOuAbrirSidenav(idElemento: string) {
    ui(`#${idElemento}`);
  },
};
