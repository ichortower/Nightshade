GAME_DIR=${HOME}/GOG Games/Stardew Valley/game
MOD_DIR=${GAME_DIR}/Mods/Nightshade

install: shaders
	dotnet build /clp:NoSummary

shaders: assets/colorizer.mgfx assets/depthoffield.mgfx

assets/colorizer.mgfx: fx/colorizer.fx
	mgfxc $< $@

assets/depthoffield.mgfx: fx/depthoffield.fx
	mgfxc $< $@

clean:
	rm -rf bin obj

uninstall:
	rm -rf "${MOD_DIR}"
