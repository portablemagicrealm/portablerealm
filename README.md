# portablerealm

Implementation of Magic Realm board game for tablets.

Requires Unity 4.6 to build.

Instructions:

How to use the map screen:

* The thing in the upper-right of the screen shows the time of day. When you want to advance to the next time, double-tap it. It will not advance if you still need to do something in the current phase, such as having selected the move action without selecting a target clearing. Also, if you are playing multiple characters, it may advance to the next character before going to the next time of day, when selecting actions during birdsong, for instance.

* In birdsong, you can swipe the icon list sideways to select the action you want to perform. The icons are the same as in RealmSpeak (used by permission). When you select the "move" icon, a clearing with a "?" is shown. To select the clearing you want to move to, scroll around the map with touch-and-drag, and double-tap on the clearing you want to move to.

* After you select an action, you can swipe down the action list to open up a new action to select.

* Actions are not tested for being valid during birdsong. If you enter an invalid action, it will just be ignored during the daylight phase.

* During the daylight phase, the action list is shown again, but you can't edit it. Instead, double-tap the highlighted action to execute it. After all your actions have been done, you can double-tap on the clock to advance to the next phase (or character).

* Also on the map screen: you can double-tap on a non-clearing area of the map to zoom in to a single tile or zoom back out. Using pinch-zoom will be implemented in the future.

* If you press and hold on a clearing, the contents of the clearing will be shown on the left. Press and hold on the clearing to restore the items. This can also be used in combat to show and target monsters in a defense box.

* Double-tapping on the character name centers their hex on the screen.