import animate;
import device;
import ui.View;
import ui.ImageView;
import ui.TextView;
import src.Tile as Tile;

/* Game constants */
var score = 0,
		high_score = 19,
		game_on = false,
		game_length = 3000,
		countdown_secs = game_length / 1000,
		lang = 'en';

exports = Class(ui.View, function (supr) {
	this.init = function (opts) {
		opts = merge(opts, {
			x: 0,
			y: 0,
			width: 320,
			height: 480,
			backgroundColor: '#37B34A'
		});

		supr(this, 'init', [opts]);

		this.build();
	};

	this.build = function () {
		this.on('app:start', bind(this, start_game_flow));

		this._scoreBoard = new ui.TextView({
			superview: this,
			x: 0,
			y: 15,
			width: 150,
			height: 50,
			size: 20,
			verticalAlign: 'middle',
			textAlign: 'center',
			multiline: false,
			color: '#fff'
		});

		this._countdown = new ui.TextView({
			superview: this._scoreBoard,
			visible: true,
			x: 110,
			y: -5,
			width: 50,
			height: 50,
			size: 24,
			color: '#fff',
			opacity: 0.7
		});

		var x_offset = 170;
		var y_offset = 5;
		var y_pad = 0;
		var field_length = 10;

		this.style.width = 480;
		this.style.height = 320;

		this._tiles = [];

		for (var row = 0; row < field_length; row++) {
			for (var col = 0; col < field_length; col++) {
				var tile = new Tile();
				tile.style.x = x_offset + col * tile.style.width;
				tile.style.y = y_offset + row * (tile.style.height + y_pad);
				this.addSubview(tile);
				this._tiles.push(tile);		
			}
		}
	};
});

/*
 * Game Play
 */

function start_game_flow() {
	var that = this;

	animate(that._scoreBoard).wait(1000).then(function () {
		that._scoreBoard.setText(text.READY);
	}).wait(1500).then(function () {
		that._scoreBoard.setText(text.SET);
	}).wait(1500).then(function () {
		that._scoreBoard.setText(text.GO);

		/* Start game */
		game_on = true;
		play_game.call(that);
	});
}

/* Game play logic */
function play_game() {
	var j = setInterval(bind(this, update_countdown), 1000);

	setTimeout(bind(this, function () {
		game_on = false;
		clearInterval(j);
		setTimeout(end_game_flow.bind(this), 1000);
		this._countdown.setText(":00");
	}), game_length);
}

function update_countdown() {
	countdown_secs -= 1;
	this._countdown.setText(":" + (("00" + countdown_secs).slice(-2)));
}

/* End game logic */
function end_game_flow() {
	this._countdown.setText('');

	this._scoreBoard.updateOpts({
		text: '',
		x: 10,
		fontSize: 17,
		verticalAlign: 'top',
		textAlign: 'left',
		multiline: true
	});

	this._scoreBoard.setText(text.END_MSG_END);

	/* Slight delay before allowing a tap reset */
	setTimeout(emit_endgame_event.bind(this), 1000);
}

function emit_endgame_event() {
	this.once('InputSelect', function() {
		this.emit('gamescreen:end');
		reset_game.call(this);
	});
}

function reset_game () {
	countdown_secs = game_length / 1000;
	this._scoreBoard.setText('');
}

var localized_strings = {
	en: {
		READY: "Ready ...",
		SET: "Set ...",
		GO: "GO!!!!!",
		MOLE: "mole",
		MOLES: "moles",
		END_MSG_START: "You whacked",
		END_MSG_END: "Tap to play again",
		HIGH_SCORE: "That's a new high score!"
	}
};

var text = localized_strings[lang.toLowerCase()];