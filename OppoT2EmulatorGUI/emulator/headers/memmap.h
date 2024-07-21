#pragma once

// Defines the Memory Locations of certain devices (This is system dependent.)

// Peripheral Controller

#define PERIPHERAL_CONTROL_MASK     1 << 1      // Mask for the peripheral controller
#define PERIPHERAL_CONTROL_ROUTINE  0x80000000  // Address of the Peripheral Control Routine
#define PERIPHERAL_CONTROL_P0       0x80000001  // Peripheral Port 0 Address
#define PERIPHERAL_CONTROL_P1       0x80000002  // Peripheral Port 1 Address
#define PERIPHERAL_CONTROL_P2       0x80000003  // Peripheral Port 2 Address
#define PERIPHERAL_CONTROL_P3       0x80000004  // Peripheral Port 3 Address
#define PERIPHERAL_CONTROL_P4       0x80000005  // Peripheral Port 4 Address
#define PERIPHERAL_CONTROL_P5       0x80000006  // Peripheral Port 5 Address
#define PERIPHERAL_CONTROL_P6       0x80000007  // Peripheral Port 6 Address
#define PERIPHERAL_CONTROL_P7       0x80000008  // Peripheral Port 7 Address

// Storage Controller

#define STORAGE_CONTROL_MASK        1 << 2      // Mask for the storage controller
#define STORAGE_CONTROL_ROUTINE     0x80000010  // Address for the Storage Control Routine
#define STORAGE_CONTROL_P0          0x80000011  // Storage Port 0 Address
#define STORAGE_CONTROL_P1          0x80000012  // Storage Port 1 Address
#define STORAGE_CONTROL_P2          0x80000013  // Storage Port 2 Address
#define STORAGE_CONTROL_P3          0x80000014  // Storage Port 3 Address

// Misc I/O
#define TTY_OUTPUT_POINTER          0x80000200  // Location of the TTY in memory


