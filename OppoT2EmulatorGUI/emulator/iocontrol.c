// Defines the basis of each I/O controller and how interrupts are handled.

#include "iocontrol.h"

#include <memory.h>
#include <stdint.h>
#include "emulator.h"
#include "memmap.h"

extern struct CPU_STATE CPU_STATE;

struct CONTROLLER_STATE {
    uint8_t peripheralFlags;
    uint8_t storageFlags;
} CONTROLLER_STATE;
struct CONTROLLER_STATE controller_state;

void input_peripheral_controller(const uint8_t port, const uint32_t data) {
    // Put data in the memory address of the port
    writeRAM(PERIPHERAL_CONTROL_P0 + port, data);

    // Set the bit for this controller in the ICR
    CPU_STATE.interrupt_source_register |= PERIPHERAL_CONTROL_MASK;

    // Signal to the controller which port requires action
    controller_state.peripheralFlags = 1 << port;

    // Signal to RAM which port requires action too
    writeRAM(PERIPHERAL_CONTROL_ROUTINE, controller_state.peripheralFlags);

    // Signal the IRQ
    CPU_STATE.irq = 1;
}