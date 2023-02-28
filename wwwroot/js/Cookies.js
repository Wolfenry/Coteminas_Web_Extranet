
window.blazorExtensions = {


    CrearCookie: function (nombre, valor, minutos) {

        var expires;

        if (minutos) {
            var date = new Date();
            date.setTime(date.getTime() + (minutos * 60 * 1000));
            expires = "; expires=" + date.toGMTString();
        }
        else {
            expires = "";
        }

        document.cookie = nombre + "=" + valor + expires + "; path=/";
    },




    BorrarCookie: function (nombre) {

        var date = new Date();
        date.setTime(date.getTime() - 60000);
        return document.cookie = nombre + '=;expires=' + date.toGMTString();

    },




    LeerCookie: function (nombre) {

        keyValue = document.cookie.match("(^|;) ?" + nombre + "=([^;]*)(;|$)");
        if (keyValue) {
            return keyValue[2];
        } else {
            return null;
        }

    }

}

