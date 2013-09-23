import ui.View;
import ui.ImageView;

exports = Class(ui.ImageView, function (supr) {
	this.init = function (opts) {
		opts = merge(opts, {
			x: 0,
			y: 0,
			image: "resources/images/main_menu.png"
		});

		supr(this, 'init', [opts]);

		this.build();
	};

	this.build = function() {
		var startButton = new ui.View({
			superview: this,
			x: 30,
			y: 112,
			width: 115,
			height: 100
		});

		startButton.on('InputSelect', bind(this, function () {
			this.emit('mainmenu:start');
		}));
	};
});