import animate;
import ui.View;
import ui.ImageView;
import ui.resource.Image as Image;

var tile_img = new Image({url: "resources/images/tile.png"});

exports = Class(ui.View, function (supr) {

	this.init = function (opts) {
		opts = merge(opts, {
			width:	tile_img.getWidth(),
			height: tile_img.getHeight()
		});

		supr(this, 'init', [opts]);

		this.build();
	};

	this.build = function () {
		var tile = new ui.ImageView({
			superview: this,
			image: tile_img,
			x: 0,
			y: 0,
			width: tile_img.getWidth(),
			height: tile_img.getHeight()
		});

		this.style.backgroundColor = "#800000"
	};
});