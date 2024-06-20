# Opcodes
This is a list of all instructions that the CPU supports. Any instruction with the `?` suffix is not fully implemented into the CPU.

Anytime `rA`, `rB`, or `rC` is seen indicates that a register is being used. `imm` signifies an immediate value, with the number following `imm` signifying the bit width.


## ADD
Adds the two numbers stored in `rB` and `rC`, then store the resulting value in `rA`.

### Syntax 
`ADD rA, rB, rC`

### Instruction Data
| Type                 | Data       |
| -------------------- | -------    |
| Clock Cycles         | 4          |
| Source               | `rB`, `rC` |
| Destination          | `rA`       |
| Instruction Format   | RRR        |


## ADDI
Adds the number in `rB` with the `simm17`, then store the resulting value in `rA`.

### Syntax 
`ADDI rA, rB, simm17`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `imm` |
| Destination          | `rA`        |
| Instruction Format   | RRI         |

## OR  
Bitwise OR the number in `rB` and `rC`, then store the resulting value in `rA`.

### Syntax
`OR rA, rB, rC`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `rC`  |
| Destination          | `rA`        |
| Instruction Format   | RRR         |


## XOR 
Bitwise XOR the number in `rB` and `rC`, then store the resulting value in `rA`.

### Syntax
`XOR rA, rB, rC`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `rC`  |
| Destination          | `rA`        |
| Instruction Format   | RRR         |


## SHLL
Shift the number in `rB` by the least significant bits in `rC` logically left, then store the resulting value in `rA`.

### Syntax
`SHLL rA, rB, rC`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `rC`  |
| Destination          | `rA`        |
| Instruction Format   | RRR         |


## SHLR
Shift the number in `rB` by the least significant bits in `rC` logically right, then store the resulting value in `rA`.

### Syntax
`SHLR, rA, rB, rC`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `rC`  |
| Destination          | `rA`        |
| Instruction Format   | RRR         |


## NEG 
Invert the bits in `rB`, then store the resulting value in `rA`.

### Syntax
`NEG rA, rB`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`        |
| Destination          | `rA`        |
| Instruction Format   | RR          |


## AND 
Bitwise AND the number in `rB` and `rC`, then store the resulting value in `rA`.

### Syntax
`AND rA, rB, rC`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `rC`  |
| Destination          | `rA`        |
| Instruction Format   | RRR         |


## LW  
Load a word from memory by adding `rB` to the `simm17` to form the memory address, then store the value into `rA`.

### Syntax
`LW rA, rB, simm17`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `imm` |
| Destination          | `rA`        |
| Instruction Format   | RRI         |


## SW  
Store a word to memory by adding `rB` to the `simm17` to form the memory address, then store the value from `rA` into memory.

### Syntax
`SW rA, rB, simm17`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `imm` |
| Destination          | `rA`        |
| Instruction Format   | RRI         |


## BRC 
Check for a given condition, then jump to the address of the `simm13` if true.

### Syntax
`BRC rA, rB, COND, simm13`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `imm` |
| Destination          | `rA`        |
| Instruction Format   | RRCI        |

### Conditions

| Integer Value | Conditions |
| ------------- | ---------- |
| 0             | `EQ`       |
| 1             | `!EQ`      |
| 2             | `GT`       |
| 3             | `GTE`      |
| 4             | `LT`       |
| 5             | `LTE`      |
| 6             | `PASS`     |

The `PASS` Conditional is always true, and if chosen will always result in a jump.


## JALR
Store the address of `Program Counter + 1` to `rA`, then jump to the address in `rB`.

### Syntax
`JALR rA, rB`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`        |
| Destination          | `rA`        |
| Instruction Format   | RR          |

## PUSH
Push the value of `rA` to the stack.

### Syntax
`PUSH rA`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rA`        |
| Destination          | `sp`        |
| Instruction Format   | R           |


## POP 
Pop the value of the stack to `rA`.

### Syntax
`POP rA`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `sp`        |
| Destination          | `rA`        |
| Instruction Format   | R           |


## LUI 
Store the value of `imm15` << 17 to `rA`.

### Syntax
`LUI rA, imm15`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `imm`       |
| Destination          | `rA`        |
| Instruction Format   | RI          |

## SIRA
Upon entering an interrupt service routine, this instruction saves the return address to `rA`.

### Syntax
`SIRA rA`

### Instruction Data

| Type                 | Data                     |
| -------------------- | -------                  |
| Clock Cycles         | 4                        |
| Source               | Interrupt Return Address |
| Destination          | `rA`                     |
| Instruction Format   | R                        |

Because interrupts can be chained together, this is a way to save the return address onto the stack. Some ISRs may not be capable of handling out of order input, and should be disabled with the [CSRW](instructions.md#csrw) instruction.

## INT
Trigger a software interrupt. (Not yet implemented into the CPU.)

### Syntax
`INT rA`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 7           |
| Source               |             |
| Destination          |             |
| Instruction Format   | R           |

Although this instruction is not supported quite yet, it will allow an interrupt to be triggered where `rA` is the value that the ISR will handle.

## ORI 
Bitwise OR the number in `rB` with `simm17`, then store the resulting value in `rA`.

### Syntax 
`ORI rA, rB, simm17`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rB`, `imm` |
| Destination          | `rA`        |
| Instruction Format   | RRI         |

## CSRW
Write the value in `rA` to the `Control Status Register`

### Syntax 
`CSRW rA`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `rA`        |
| Destination          | `csr`       |
| Instruction Format   | RRI         |

## CSRR
Read the value in the `Control Status Register` and store in `rA`

### Syntax 
`CSRW rA`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 4           |
| Source               | `csr`       |
| Destination          | `rA`        |
| Instruction Format   | RRI         |

## Psuedo Opcodes

The assembler supports extra psuedo opcodes, which simplify code writing. These are combinations of other instructions.

## MOVI

Move a 32-bit immediate value to a register.

### Syntax
`MOVI rA, imm32`

### Instruction Data

| Type                 | Data        |
| -------------------- | -------     |
| Clock Cycles         | 8           |
| Source               | `imm32`     |
| Destination          | `rA`        |

This uses the [`ori`](#ori) and [`lui`](#lui) opcodes to move a value in two instructions.

```
    lui rA, (imm32 & 0xFFFE0000)
    ori rA, (imm32 & 0x0001FFFF)
```