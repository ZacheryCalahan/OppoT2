# Instruction Set Overview

| Type           | Instruction                                   | Syntax                   | Type | Function                          |
| -------------- | --------------------------------------------- | ------------------------ | ---- | --------------------------------- |
| ALU            | [ADD](../cpu/instructions.md#add)             | ADD rA, rB, rC           | RRR  | rA <- rB + rC                     |
|                | [ADDI](../cpu/instructions.md#addi)           | ADDI rA, rB, simm17      | RRI  | rA <- rB + simm17                 |
|                | [OR](../cpu/instructions.md#or)               | OR rA, rB, rC            | RRR  | rA <- rB \| rC                    |
|                | [XOR](../cpu/instructions.md#xor)             | XOR rA, rB, rC           | RRR  | rA <- rB ^ rC                     |
|                | [SHLL](../cpu/instructions.md#shll)           | SHLL rA, rB, rC          | RRR  | rA <- rB << rC                    |
|                | [SHLR](../cpu/instructions.md#shlr)           | SHLR, rA, rB, rC         | RRR  | rA <- rB >> rC                    |
|                | [NEG](../cpu/instructions.md#neg)             | NEG rA, rB               | RR   | rA <- ~rB                         |
|                | [AND](../cpu/instructions.md#and)             | AND rA, rB, rC           | RRR  | rA <- rB & rC                     |
|                |                                               |                          |      |                                   |
| Memory         | [LW](../cpu/instructions.md#lw)               | LW rA, rB, simm17        | RRI  | R[regA] <- Mem[ R[regB] + immed ] |
|                | [SW](../cpu/instructions.md#sw)               | SW rA, rB, simm17        | RRI  | R[regA] -> Mem[ R[regB] + immed ] |
|                |                                               |                          |      |                                   |
| Jumps          | [BRC](../cpu/instructions.md#brc)             | BRC rA, rB, COND, simm13 | RRCI | if (rA compares rB) jmp           |
|                | [JALR](../cpu/instructions.md#jalr)           | JALR rA, rB              | RR   | rA <- PC + 1 && PC <- rB          |
|                |                                               |                          |      |                                   |
| Stack          | [PUSH](../cpu/instructions.md#push)           | PUSH rA                  | R    | stack <- rA                       |
|                | [POP](../cpu/instructions.md#pop)             | POP rA                   | R    | rA <- stack                       |
|                |                                               |                          |      |                                   |
| Immediate      | [LUI](../cpu/instructions.md#lui)             | LUI rA, imm15            | RI   | rA <- imm15 << 17                 |
|                |                                               |                          |      |                                   |
| Interrupt      | [SIRA](../cpu/instructions.md#sira)           | SIRA rA                  | RR   | rA <- Int Return Address          |
|                | [INT?](../cpu/instructions.md#int)           | INT, rA, simm17          |      | Trigger a software interrupt      |
|                | [ORI](../cpu/instructions.md#ori)             | ORI, rA, rB, simm17      | RRI  | rA <- rB \| imm17                 |
|                |                                               |                          |      |                                   |
| Control Status | [CSRW](../cpu/instructions.md#csrw)           | CSRW rA                  | R    | rA -> CSR                         |
|                | [CSRR](../cpu/instructions.md#csrr)           | CSRR rA                  | R    | rA <- CSR                         |