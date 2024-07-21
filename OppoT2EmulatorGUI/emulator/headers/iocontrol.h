#pragma once
#include <stdint.h>

// Emulate the data flow of a controller receiving a packet of data.
void input_peripheral_controller(uint8_t port, uint32_t data);
