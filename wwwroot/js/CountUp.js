function countup(){
    $('.counter').each(function () {
        var $this = $(this),
            countTo = $this.attr('data-to');

        $({ countNum: $this.attr('data-from') }).animate({
            countNum: countTo
        },

            {

                duration: 1000,
                easing: 'linear',
                step: function () {
                    $this.text(Math.floor(this.countNum));
                },
                complete: function () {
                    $this.text(this.countNum);
                }

            });

    });


    $('.counterfloat').each(function () {
        var a = $(this),
            countTo = a.attr('data-to');

        $({ countNum: a.attr('data-from') }).animate({
            countNum: countTo
        },

            {

                duration: 1000,
                easing: 'linear',
                step: function () {
                    $this.text(Math.floor(this.countNum));
                },
                complete: function () {
                    $this.text(this.countNum);
                }

            });

    });
}

function countupfloat() {
    $('.counterfloat').each(function () {
        var size = $(this).text().split(".")[1] ? $(this).text().split(".")[1].length : 0;
        $(this).prop('counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 1000,
            easing: 'linear',
            step: function (now) {
                $(this).text(parseFloat(now).toFixed(size));
            }
        });
    });
}	