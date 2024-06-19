# Instructions

| Type           | Instruction | Syntax                   | Type | Function                          |
| -------------- | ----------- | ------------------------ | ---- | --------------------------------- |
| ALU            | ADD         | ADD rA, rB, rC           | RRR  | rA <- rB + rC                     |
|                | ADDI        | ADDI rA, rB, simm17      | RRI  | rA <- rB + simm17                 |
|                | OR          | OR rA, rB, rC            | RRR  | rA <- rB \| rC                    |
|                | XOR         | XOR rA, rB, rC           | RRR  | rA <- rB ^ rC                     |
|                | SHLL        | SHLL rA, rB, rC          | RRR  | rA <- rB << rC                    |
|                | SHLR        | SHLR, rA, rB, rC         | RRR  | rA <- rB >> rC                    |
|                | NEG         | NEG rA, rB               | RR   | rA <- ~rB                         |
|                | AND         | AND rA, rB, rC           | RRR  | rA <- rB & rC                     |
|                |             |                          |      |                                   |
| Memory         | LW          | LW rA, rB, simm17        | RRI  | R[regA] <- Mem[ R[regB] + immed ] |
|                | SW          | SW rA, rB, simm17        | RRI  | R[regA] -> Mem[ R[regB] + immed ] |
|                |             |                          |      |                                   |
| Jumps          | BRC         | BRC rA, rB, COND, simm13 | RRCI | if (rA compares rB) jmp           |
|                | JALR        | JALR rA, rB              | RR   | rA <- PC + 1 && PC <- rB          |
|                |             |                          |      |                                   |
| Stack          | PUSH        | PUSH rA                  | R    | stack <- rA                       |
|                | POP         | POP rA                   | R    | rA <- stack                       |
|                |             |                          |      |                                   |
| Immediate      | LUI         | LUI rA, imm15            | RI   | rA <- imm15 << 17                 |
|                |             |                          |      |                                   |
| Interrupt      | SIRA        | SIRA rA                  | RR   | rA <- Int Return Address          |
|                | INT?        | INT, rA, simm17          |      | Trigger a software interrupt      |
|                | ORI         | ORI, rA, rB, simm17      | RRI  | rA <- rB \| imm17                 |
|                |             |                          |      |                                   |
| Control Status | CSRW        | CSRW rA                  | R    | rA -> CSR                         |
|                | CSRR        | CSRR rA                  | R    | rA <- CSR                         |