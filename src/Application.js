import device;
import ui.StackView as StackView;

import src.TitleScreen as TitleScreen;
import src.MainMenu as MainMenu;
import src.GameScreen as GameScreen;

exports = Class(GC.Application, function () {

	this.initUI = function () {
		var titleScreen = new TitleScreen();
		var mainMenu = new MainMenu();
		var gameScreen = new GameScreen();

		this.view.style.backgroundColor = '#30B040';

		var rootView = new StackView({
			superview: this,
			x: device.width / 2 - 240,
			y: device.height / 2 - 160,
			width: 480,
			height: 320,
			clip: true,
			backgroundColor: '#c0c0c0'
		});

		rootView.push(titleScreen);

		titleScreen.on('titlescreen:start', function () {
			rootView.push(mainMenu);
		});

		mainMenu.on('mainmenu:start', function () {
			rootView.push(gameScreen);
			gameScreen.emit('app:start');
		});

		gameScreen.on('gamescreen:end', function () {
			rootView.pop();
		});
	};
	
	this.launchUI = function () {};
});
