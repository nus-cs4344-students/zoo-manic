import ui.View;
import ui.ImageView;

exports = Class(ui.ImageView, function (supr) {
	this.init = function (opts) {
		opts = merge(opts, {
			x: 0,
			y: 0,
			image: "resources/images/title_screen.png"
		});

		supr(this, 'init', [opts]);

		this.build();
	};

	this.build = function() {
		var startButton = new ui.View({
			superview: this,
			x: 0,
			y: 0,
			width: 480,
			height: 320
		});

		startButton.on('InputSelect', bind(this, function () {
			this.emit('titlescreen:start');
		}));
	};
});