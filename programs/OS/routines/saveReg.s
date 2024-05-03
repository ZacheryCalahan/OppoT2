# Routine that saves all saved registers.

.saveReg
    push s0
    push s1
    push s2
    push s3
    push s4
    push s5
    push s6
    push s7
    jalr r0, ra

.retReg
    pop s7
    pop s6
    pop s5
    pop s4
    pop s3
    pop s2
    pop s1
    pop s0
    jalr r0, ra
    
