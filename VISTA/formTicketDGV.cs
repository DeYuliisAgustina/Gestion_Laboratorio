using Controladora;
using Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace VISTA
{
    public partial class formTicketDGV : Form
    {
        public formTicketDGV()
        {
            InitializeComponent();
            ActualizarGrilla();
            dgvTicket.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //con esto hago que las columnas se ajusten al contenido
        }

        //metodo para mover la ventana
        [DllImport("User32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("User32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void ActualizarGrilla()
        {
            dgvTicket.DataSource = null;
            dgvTicket.DataSource = ControladoraSede.Instancia.RecuperarSedes(); // Recupero las sedes de la base de datos y las muestro en la grilla 
            dgvTicket.DataSource = ControladoraComputadora.Instancia.RecuperarComputadoras();
            dgvTicket.DataSource = ControladoraLaboratorio.Instancia.RecuperarLaboratorios();
            dgvTicket.DataSource = ControladoraTecnico.Instancia.RecuperarTecnicos();
            dgvTicket.DataSource = ControladoraTicket.Instancia.RecuperarTicket();
            dgvTicket.Columns["Laboratorios"].Visible = false;

        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void formHistorialDGV_Load(object sender, EventArgs e)
        {
            ActualizarGrilla(); // Actualizo la grilla para que muestre todos los tickets  
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Form formTicketAM = new formTicketAM();
            formTicketAM.ShowDialog();
            ActualizarGrilla();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTicket.Rows.Count > 0) //si hay tickets en la grilla entonces se puede eliminar un ticket 
            {
                var ticketSelecionado = (Ticket)dgvTicket.CurrentRow.DataBoundItem; // Recupero el ticket seleccionado de la grilla y lo guardo en una variable de tipo Ticket para luego eliminarlo

                var confirmacion = MessageBox.Show("¿Está seguro que desea eliminar el ticket?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes) // Si se confirma la eliminación del ticket, se elimina de la base de datos y se actualiza la grilla
                {
                    var respuesta = ControladoraTicket.Instancia.EliminarTicket(ticketSelecionado);
                    MessageBox.Show(respuesta, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarGrilla();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un ticket para eliminarlo.");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvTicket.Rows.Count > 0) //si hay tickets en la grilla entonces se puede modificar un ticket
            {
                var ticketSeleccionado = (Ticket)dgvTicket.CurrentRow.DataBoundItem; // Recupero el ticket seleccionado de la grilla y lo guardo en una variable de tipo Ticket para luego modificarlo  
                formTicketAM formTicketAM = new formTicketAM(ticketSeleccionado); //aca le paso el ticket seleccionado a la ventana de modificacion de ticket para que se carguen los datos del ticket seleccionado en los campos de texto
                formTicketAM.ShowDialog(); 
            }
            else
            {
                MessageBox.Show("Seleccione un ticket para modificarlo.");
            }
            ActualizarGrilla();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtBuscarTicket.Text != "Por código de pc, técnico, sede o laboratorio")
            {
                var listaTicket = ControladoraTicket.Instancia.RecuperarTicket();
                var ticketEncontrado = listaTicket.Where(t => t.Computadora.CodigoComputadora.ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.TicketId.ToString().Contains(txtBuscarTicket.Text.ToLower()) || t.estado.ToString().ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.FechaCreacion.ToString().ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.tipo.ToString().ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.Tecnico.NombreyApellido.ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.NombreSede.ToLower().Contains(txtBuscarTicket.Text.ToLower()) || t.Computadora.Laboratorio.NombreLaboratorio.ToLower().Contains(txtBuscarTicket.Text.ToLower()));

                if (ticketEncontrado.Count() > 0) // Si se encontraron tickets, mostrarlos en la grilla 
                {
                    dgvTicket.DataSource = null; // Limpio la grilla cuando se realiza una búsqueda para que no se dupliquen los tickets
                    dgvTicket.DataSource = ticketEncontrado.ToList(); // Muestro los tickets encontrados en la grilla
                    dgvTicket.Columns["Laboratorios"].Visible = false; // Oculto la columna Laboratorios de la grilla

                }
                else
                {
                    MessageBox.Show("No se han encontrado tickets."); // Si no se encontraron tickets, muestro un mensaje de advertencia
                    ActualizarGrilla(); // Actualizo la grilla para que muestre todos los tickets 
                    dgvTicket.Columns["Laboratorios"].Visible = false;

                }
            }
            else
            {
                MessageBox.Show("Ingrese un codigo de computadora, técnico, sede o laboratorio para buscar el ticket.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarGrilla(); // Actualizo la grilla para que muestre todos los tickets 
                dgvTicket.Columns["Laboratorios"].Visible = false;

            }
        }

        private void txtBuscarTicket_Enter(object sender, EventArgs e)
        {
            if (txtBuscarTicket.Text == "Por código de pc, técnico, sede o laboratorio")
            {
                txtBuscarTicket.Text = "";
                txtBuscarTicket.ForeColor = Color.Black;
            }
        }

        private void txtBuscarTicket_Leave(object sender, EventArgs e)
        {
            if (txtBuscarTicket.Text == "")
            {
                txtBuscarTicket.Text = "Por código de pc, técnico, sede o laboratorio";
                txtBuscarTicket.ForeColor = Color.Silver;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // Verificar si hay tickets creados
            if (ControladoraTicket.Instancia.RecuperarTicket().Count == 0) // Si no hay tickets creados, mostrar un mensaje de advertencia 
            {
                MessageBox.Show("No hay tickets creados.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog(); // Creo un objeto de tipo SaveFileDialog para guardar el PDF 
            guardar.FileName = "Historial de Tickets" + ".pdf"; // Nombre del archivo PDF que se va a guardar

            string paginahtml_texto = Properties.Resources.plantilla.ToString(); //Cargo la plantilla HTML en una variable string para reemplazar los valores de los tickets en el PDF que se va a crear

            if (guardar.ShowDialog() == DialogResult.OK) //Si selecciona si en el dialogo de guardar archivo, entonces se crea el PDF
            {
                try
                {
                    using (FileStream stream = new FileStream(guardar.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) //guardo el pdf, uso create para crear un archivo nuevo, write para escribir en el archivo y readwrite para leer y escribir en el archivo 
                    {
                        using (Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25)) //Creo el documento PDF y le doy un tamaño de hoja A4
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream); //writer se encarga de escribir en el archivo PDF, pdfDoc es PDF que se va a crear y stream es el PDF que se va a guardar 

                            pdfDoc.Open(); // Abro el documento PDF

                            var listaTicket = ControladoraTicket.Instancia.RecuperarTicket(); // Recupero los tickets de la base de datos

                            foreach (var ticket in listaTicket) //Recorro la lista de tickets y muestro los valores de los tickets en el PDF
                            {
                                string paginahtml_ticket = paginahtml_texto; // Cargo la plantilla HTML en una variable string para luego reemplazar los valores de los tickets en el PDF 

                                paginahtml_ticket = paginahtml_ticket.Replace("@TecnicoId", ticket.TecnicoId.ToString()); // Reemplazo los valores de los tickets en la plantilla HTML
                                paginahtml_ticket = paginahtml_ticket.Replace("@TicketId", ticket.TicketId.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@ComputadoraId", ticket.ComputadoraId.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@Computadora", ticket.Computadora.CodigoComputadora.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@Ubicacion", ticket.Ubicacion.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@NombreSede", ticket.NombreSede.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@FechaCreacion", ticket.FechaCreacion.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@DescripcionTicket", ticket.DescripcionTicket.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@Tecnico", ticket.Tecnico.NombreyApellido.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@categoria", ticket.categoria.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@urgencia", ticket.urgencia.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@estado", ticket.estado.ToString());
                                paginahtml_ticket = paginahtml_ticket.Replace("@tipo", ticket.tipo.ToString());

                                var listaTecnicos = ControladoraTecnico.Instancia.RecuperarTecnicos();
                                foreach (var tecnico in listaTecnicos) 
                                {
                                    if (tecnico.TecnicoId == ticket.TecnicoId) // Si el id del técnico es igual al id del técnico del ticket, muestro el nombre y apellido del técnico y la cantidad de tickets que tiene asignados en el PDF
                                    {
                                        paginahtml_ticket = paginahtml_ticket.Replace("@Tecnico", tecnico.NombreyApellido.ToString());
                                        paginahtml_ticket = paginahtml_ticket.Replace("@CantidadTickets", tecnico.Tickets.Count.ToString()); // Lo que hago con count es contar la cantidad de tickets que tiene asignados a cada técnico y mostrarlo en el PDF
                                    }
                                }

                                using (StringReader sr = new StringReader(paginahtml_ticket)) //Convierto el string en un archivo HTML  
                                {
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr); //Parseo el archivo HTML a PDF
                                }
                            }
                           
                            pdfDoc.Close(); 
                        }
                        stream.Close();
                    }

                    MessageBox.Show("Archivo PDF creado con éxito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abrir el archivo PDF creado
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() //Abro el archivo PDF con el visor de PDF predeterminado del sistema operativo 
                    {
                        FileName = guardar.FileName, // Nombre del archivo PDF que se va a abrir 
                        UseShellExecute = true, // Uso de la shell del sistema operativo para abrir el archivo PDF, la shell es el entorno de usuario gráfico del sistema operativo
                        Verb = "open" // Acción que se va a realizar con el archivo PDF, en este caso abrirlo
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el archivo PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void formTicketDGV_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void txtBuscarTicket_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressSoloLetras(e, txtBuscarTicket.Text).Handled)
            {
                MessageBox.Show("Solo se permiten letras y números, no caracteres especiales.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static public KeyPressEventArgs KeyPressSoloLetras(KeyPressEventArgs e, string TEXTO)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
            return e;
        }
    }
}
