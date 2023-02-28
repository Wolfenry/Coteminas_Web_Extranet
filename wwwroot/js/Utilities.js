function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function rotate() {
    $(".rotate").click(function () {
        $(this).toggleClass("down");
    })
}

//////////DRIVER TUTORIAL
function runDriver() {

    const driver = new Driver({
        className: 'driver', // className to wrap driver.js popover
        animate: true,  // Animate while changing highlighted element
        opacity: 0.75,  // Background opacity (0 means only popovers and without overlay)
        padding: 10,    // Distance of element from around the edges
        allowClose: false, // Whether clicking on overlay should close or not
        overlayClickNext: true,// Should it move to next step on overlay click
        doneBtnText: 'Hecho', // Text on the final button
        closeBtnText: 'Cerrar', // Text on the close button for this step
        nextBtnText: 'Siguiente', // Next button text for this step
        prevBtnText: 'Anterior', // Previous button text for this step
        showButtons: true, // Do not show control buttons in footer
        keyboardControl: true, // Allow controlling through keyboard (escape to close, arrow keys to move)
    });

    //$(this).closest('table').find(' tbody tr:first').attr('id','r1');

    driver.defineSteps([
        {
            element: '#cog',
            popover: {
                title: 'Bienvenido al tutorial !',
                description: 'Los comandos generales ahora se encuentran en el desplegable al tocar la tuerca.',
                position: 'right'
            }
        },
        {
            element: '#colvis',
            popover: {
                title: 'Nuevas funciones',
                description: 'Permite personalizar la visibilidad de las columnas.',
                position: 'top'
            }
        },
        {
            element: '#est',
            popover: {
                title: 'Cambio de paradigma',
                description: 'Los comandos de las filas ahora se encuentran haciendole <b>DOBLE CLICK</b> a las mismas.',
                position: 'left'
            }
        },
        {
            element: '#bgeneral',
            popover: {
                title: 'Mas dinámico',
                description: 'Busqueda general de datos.',
                position: 'left'
            }
        },
        {
            element: '#periodo',
            popover: {
                title: 'Mas rápido',
                description: 'Los datos se encuentran filtrados por período.',
                position: 'left'
            }
        },
    ]);

    driver.start();


}

function setContrasteWidth() {
    var navwidth = $(".navbar-nav");
    $("#contrasteopciones").css("width", (navwidth.width() + 50) + "px");
}