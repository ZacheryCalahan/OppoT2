#include <stdio.h>
#include <stdlib.h>
#include "emulator.h"
#include <time.h>
#include <windows.h>
#include <gtk/gtk.h>
#include <gdk/gdk.h>
#include "main.h"
#include <interface.h>

extern struct CPU_STATE CPU_STATE;

HANDLE StartEmulator();

GtkWidget *serial_output;
GtkTextBuffer *serial_output_buffer;
boolean CLK_STEP = FALSE;
boolean CLK_APROP = FALSE;
struct REG_VALUES {
    GtkWidget *r0;

    GtkWidget *ra;

    GtkWidget *s0;
    GtkWidget *s1;
    GtkWidget *s2;
    GtkWidget *s3;
    GtkWidget *s4;
    GtkWidget *s5;
    GtkWidget *s6;
    GtkWidget *s7;

    GtkWidget *t0;
    GtkWidget *t1;
    GtkWidget *t2;
    GtkWidget *t3;
    GtkWidget *t4;
    GtkWidget *t5;
    GtkWidget *t6;
    GtkWidget *t7;

    GtkWidget *fa0;
    GtkWidget *fa1;
    GtkWidget *fa2;
    GtkWidget *fa3;
    GtkWidget *fr;

    GtkWidget *r23;
    GtkWidget *r24;
    GtkWidget *r25;
    GtkWidget *r26;
    GtkWidget *r27;
    GtkWidget *r28;
    GtkWidget *r29;

    GtkWidget *istat;
    GtkWidget *sp;

    GtkWidget *pc;
    GtkWidget *csr;
    GtkWidget *intsr;
} REG_VALUES;

static void quit_cb(GtkWindow *window) {
    gtk_window_close(window);
}

static gboolean key_press_event_cb(GdkEvent *event, guint keyval, guint keycode, GdkModifierType state, gpointer user_data) {
    const char *key_name = gdk_keyval_name(keyval);
    const uint32_t c = gdk_keyval_to_unicode(keyval);

    // Ensure character is an ASCII character.
    if (c < 256) {
        HandleKeyboard(c);
    }

    return true;
}

static void clock_toggle_cb(GtkToggleButton *button, gpointer data) {
    CLK_APROP = gtk_toggle_button_get_active(button);
}

static void clock_step_cb(GtkButton *button, gpointer data) {
    CLK_STEP = TRUE;
}

void update_register_values() {
    // This seems terrible, and could probably be done with a loop,
    // but I simply don't care enough about something that is fast and
    // can be folded away in the editor. :D

    char buffer[20];

    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[0]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r0), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[1]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.ra), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[2]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s0), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[3]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s1), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[4]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s2), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[5]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s3), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[6]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s4), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[7]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s5), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[8]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s6), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[9]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.s7), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[10]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t0), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[11]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t1), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[12]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t2), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[13]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t3), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[14]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t4), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[15]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t5), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[16]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t6), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[17]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.t7), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[18]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.fa0), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[19]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.fa1), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[20]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.fa2), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[21]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.fa3), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[22]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.fr), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[23]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r23), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[24]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r24), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[25]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r25), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[26]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r26), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[27]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r27), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[28]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r28), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[29]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.r29), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[30]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.istat), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[31]);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.sp), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.pc);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.pc), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.csr);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.csr), buffer);
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.interrupt_source_register);
    gtk_label_set_text(GTK_LABEL(REG_VALUES.intsr), buffer);
}

void write_to_tty(const char c) {
    gtk_text_buffer_insert_at_cursor(serial_output_buffer, &c, 1);
}

static void activate(GtkApplication *app, gpointer user_data) {
    // Init builder
    GtkBuilder *builder = gtk_builder_new_from_file("../window.ui");

    // Get window
    GtkWidget *window = GTK_WIDGET(gtk_builder_get_object(builder, "window"));
    gtk_window_set_application(GTK_WINDOW(window), app);
    g_signal_connect(window, "close-request", G_CALLBACK(quit_cb), NULL);

    // CSS things.
    GtkCssProvider *provider = gtk_css_provider_new();
    GFile *file = g_file_new_for_path("../colorizer.css");
    gtk_css_provider_load_from_file(provider, file);
    gtk_style_context_add_provider_for_display(gdk_display_get_default (),
        GTK_STYLE_PROVIDER (provider), GTK_STYLE_PROVIDER_PRIORITY_APPLICATION);

    // Get Clock Buttons
    GtkWidget *clock_prop_button = GTK_WIDGET(gtk_builder_get_object(builder, "clock_prop_button"));
    GtkWidget *clock_step_button = GTK_WIDGET(gtk_builder_get_object(builder, "clock_step_button"));
    g_signal_connect(clock_step_button, "clicked", G_CALLBACK(clock_step_cb), NULL);
    g_signal_connect(clock_prop_button, "clicked", G_CALLBACK(clock_toggle_cb), NULL);

    // Get Serial Output
    GtkWidget *serial_output = GTK_WIDGET(gtk_builder_get_object(builder, "serial_output"));
    serial_output_buffer = gtk_text_buffer_new(NULL);
    gtk_text_view_set_buffer(GTK_TEXT_VIEW(serial_output), serial_output_buffer);
    gtk_text_view_set_editable(GTK_TEXT_VIEW(serial_output), FALSE);

    // Event handler for the keyboard input
    GtkEventControllerKey *keyboard_handler = GTK_EVENT_CONTROLLER_KEY(gtk_event_controller_key_new());
    g_signal_connect(keyboard_handler, "key-pressed", G_CALLBACK(key_press_event_cb), NULL);
    gtk_widget_add_controller(window, GTK_EVENT_CONTROLLER(keyboard_handler));

    // Set up the struct of register values
    REG_VALUES.r0 = GTK_WIDGET(gtk_builder_get_object(builder, "r0_value"));
    REG_VALUES.ra = GTK_WIDGET(gtk_builder_get_object(builder, "ra_value"));

    REG_VALUES.s0 = GTK_WIDGET(gtk_builder_get_object(builder, "s0_value"));
    REG_VALUES.s1 = GTK_WIDGET(gtk_builder_get_object(builder, "s1_value"));
    REG_VALUES.s2 = GTK_WIDGET(gtk_builder_get_object(builder, "s2_value"));
    REG_VALUES.s3 = GTK_WIDGET(gtk_builder_get_object(builder, "s3_value"));
    REG_VALUES.s4 = GTK_WIDGET(gtk_builder_get_object(builder, "s4_value"));
    REG_VALUES.s5 = GTK_WIDGET(gtk_builder_get_object(builder, "s5_value"));
    REG_VALUES.s6 = GTK_WIDGET(gtk_builder_get_object(builder, "s6_value"));
    REG_VALUES.s7 = GTK_WIDGET(gtk_builder_get_object(builder, "s7_value"));

    REG_VALUES.t0 = GTK_WIDGET(gtk_builder_get_object(builder, "t0_value"));
    REG_VALUES.t1 = GTK_WIDGET(gtk_builder_get_object(builder, "t1_value"));
    REG_VALUES.t2 = GTK_WIDGET(gtk_builder_get_object(builder, "t2_value"));
    REG_VALUES.t3 = GTK_WIDGET(gtk_builder_get_object(builder, "t3_value"));
    REG_VALUES.t4 = GTK_WIDGET(gtk_builder_get_object(builder, "t4_value"));
    REG_VALUES.t5 = GTK_WIDGET(gtk_builder_get_object(builder, "t5_value"));
    REG_VALUES.t6 = GTK_WIDGET(gtk_builder_get_object(builder, "t6_value"));
    REG_VALUES.t7 = GTK_WIDGET(gtk_builder_get_object(builder, "t7_value"));

    REG_VALUES.fa0 = GTK_WIDGET(gtk_builder_get_object(builder, "fa0_value"));
    REG_VALUES.fa1 = GTK_WIDGET(gtk_builder_get_object(builder, "fa1_value"));
    REG_VALUES.fa2 = GTK_WIDGET(gtk_builder_get_object(builder, "fa2_value"));
    REG_VALUES.fa3 = GTK_WIDGET(gtk_builder_get_object(builder, "fa3_value"));
    REG_VALUES.fr = GTK_WIDGET(gtk_builder_get_object(builder, "fr_value"));

    REG_VALUES.r23 = GTK_WIDGET(gtk_builder_get_object(builder, "r23_value"));
    REG_VALUES.r24 = GTK_WIDGET(gtk_builder_get_object(builder, "r24_value"));
    REG_VALUES.r25 = GTK_WIDGET(gtk_builder_get_object(builder, "r25_value"));
    REG_VALUES.r26 = GTK_WIDGET(gtk_builder_get_object(builder, "r26_value"));
    REG_VALUES.r27 = GTK_WIDGET(gtk_builder_get_object(builder, "r27_value"));
    REG_VALUES.r28 = GTK_WIDGET(gtk_builder_get_object(builder, "r28_value"));
    REG_VALUES.r29 = GTK_WIDGET(gtk_builder_get_object(builder, "r29_value"));

    REG_VALUES.istat = GTK_WIDGET(gtk_builder_get_object(builder, "istat_value"));
    REG_VALUES.sp = GTK_WIDGET(gtk_builder_get_object(builder, "sp_value"));
    REG_VALUES.pc = GTK_WIDGET(gtk_builder_get_object(builder, "pc_value"));
    REG_VALUES.csr = GTK_WIDGET(gtk_builder_get_object(builder, "csr_value"));
    REG_VALUES.intsr = GTK_WIDGET(gtk_builder_get_object(builder, "intsr_value"));






    // Finish up build
    gtk_window_present(GTK_WINDOW(window));
    g_object_unref(builder);

}

int main(int argc, char *argv[]) {
    // Start the emulator and load memory. (Make the init a file chooser later.)

    // ALSO, you already know my name from my GitHub. Don't flame me for the hardcoded file path, you're just being
    // a prude.

    FILE *fptr = fopen("C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\out\\out.o", "r");
    init();
    read_bin_to_memory(fptr);

    HANDLE emulatorThread = StartEmulator();

    // Create the GUI

#ifdef GTK_SRCDIR
    g_chdir (GTK_SRCDIR)
#endif

    GtkApplication *app = gtk_application_new("org.gtk.example", G_APPLICATION_DEFAULT_FLAGS);
    g_signal_connect(app, "activate", G_CALLBACK(activate), NULL);

    int status = g_application_run (G_APPLICATION (app), argc, argv);
    g_object_unref(app);

    return status;

}

// Emulator Stuff

#define CLK_SPEED 0
DWORD WINAPI EmulatorThread(void* data) {
    Sleep(1000); // Sleep for 1 second to wait for the GUI to init.
    // This is a thread with the emulator.
    clock_t start = clock();

    while (1) {
        // Run a cycle @ clock speed

        const int elapsed = clock() - start;
        if (elapsed >= CLK_SPEED && CLK_APROP) {
            // Auto Propagate mode
            emulate_cycle();
            start = clock();
        } else if (CLK_STEP) {
            // Clock Step Mode
            emulate_cycle();
            CLK_STEP = FALSE;
            update_register_values();
        }
    }
}

// Start the thread of the Emulator, and return its handler.
HANDLE StartEmulator() {
    // Start the emulator.
    return CreateThread(NULL, 0, EmulatorThread, NULL, 0, NULL);
}




