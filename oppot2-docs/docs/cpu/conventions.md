# CPU Conventions

This CPU has been designed to used in various different ways, but the conventions will allow developers to keep track of the CPU state.

## Registers

The registers are of a similar design to the RISC-V processor, but are organized differently. The names of the registers should be used to refer to a register within the assembler, but the assembler can also accept the register's integer representation. 

The registers that the CPU keeps track of are in two different groups, the general purpose registers and the special registers. 

### General Purpose Registers

These registers can be used in any instruction that handles registers specifically. Each register's use is only by convention, and other than `r0` are not handled by the CPU in any special way. 

!!! warning "Register Safety"

    A developer can write a return address of a subroutine to the stack pointer without issue from the CPU if that is <i>really</i> what the developer wants. It is heavily recommended that a user uses the register names to avoid such bugs.

| Register Integer | Register Name           | Usage                | Saved by calle- |
| ---------------- | ----------------------- | -------------------- | --------------- |
| 0                | `r0`                    | Hardwired Zero       |                 |
| 1                | `ra`                    | Return Address       | \-R             |
| 2                | `s0`                    | Saved Register       | \-E             |
| 3                | `s1`                    | Saved Register       | \-E             |
| 4                | `s2`                    | Saved Register       | \-E             |
| 5                | `s3`                    | Saved Register       | \-E             |
| 6                | `s4`                    | Saved Register       | \-E             |
| 7                | `s5`                    | Saved Register       | \-E             |
| 8                | `s6`                    | Saved Register       | \-E             |
| 9                | `s7`                    | Saved Register       | \-E             |
| 10               | `t0`                    | Temp Register        | \-R             |
| 11               | `t1`                    | Temp Register        | \-R             |
| 12               | `t2`                    | Temp Register        | \-R             |
| 13               | `t3`                    | Temp Register        | \-R             |
| 14               | `t4`                    | Temp Register        | \-R             |
| 15               | `t5`                    | Temp Register        | \-R             |
| 16               | `t6`                    | Temp Register        | \-R             |
| 17               | `t7`                    | Temp Register        | \-R             |
| 18               | `r18`                   | Function Argument 0  | \-E             |
| 19               | `r19`                   | Function Argument 1  | \-E             |
| 20               | `r20`                   | Function Argument 2  | \-E             |
| 21               | `r21`                   | Function Argument 3  | \-E             |
| 22               | `r22`                   | Function Return      | \-E             |
| 23               | `r23`                   |                      |                 |
| 24               | `r24`                   |                      |                 |
| 25               | `r25`                   |                      |                 |
| 26               | `r26`                   |                      |                 |
| 27               | `r27`                   |                      |                 |
| 28               | `r28`                   |                      |                 |
| 29               | `r29`                   | Long Jump Register   |                 |
| 30               | `isr`                   | Interrupt Status Reg | \-E             |
| 31               | `r31`                   | Stack Pointer        |                 |

### Special Registers

Some registers do not live within the Register File, and are only accessable through special instructions. These are listed here.

| Register Name | Use                  | Accessable?                                   |
| ------------- | -------------------- | --------------------------------------------- |
| `pc`          | Program Counter      | Yes [BRC](../cpu/instructions.md#brc)         |
| `csr`         | Return Address       | Yes [CSRW](../cpu/instructions.md#csrw)       |
| `iret`        | Return Address       | Read-Only [SIRA](../cpu/instructions.md#sira) |

## Memory

Generally the memory map can be entirely chosen by the developer, but a few memory locations are hardcoded. These exceptions are listed below.

| Memory Location | Use                              | Description 
| --------------- | -------------------------------- | ----------------------------------------------------------- | 
| 0x00000000      | Boot Location                    | Location CPU starts execution at.                           |
| 0x00002FFF      | Interrupt Service Routine Vector | Location of where CPU jumps to when an interrupt is raised. |

Because the boot code could possibly be larger than 12287 words, the start of the ISR vector, it is recommended that a memory address is jumped to directly after gaining control of the CPU. Example (Kernel location 0x00004fff):

```
    movi r29, 0x00004ffff   # Load kernel location
    jalr r0, r29            # Long Jump to kernel
```

!!! note
    [MOVI](../cpu/instructions.md#movi) is a psuedo-op, and uses two instructions to load a 32-bit value into a register.

This is an example of the maximum needed words to jump to any location in memory, which requires 3 words. Since there are 12287 words before the start of the next reserved location in memory, this area is suitable for BIOS style ROM as well.
