#include "interface.h"

#include <iocontrol.h>

// Emulate the keyboard
void HandleKeyboard(const char c) {
    // TODO: Change this so it takes in a scan code, not a char.
    input_peripheral_controller(0, c);
}

