﻿using Controladora;
using Entidades;
using System.Runtime.InteropServices;

namespace VISTA
{
    public partial class formTecnicoDGV : Form
    {
        public formTecnicoDGV()
        {
            InitializeComponent();
            ActualizarGrilla();
            dgvTecnico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //con esto hago que las columnas se ajusten al contenido
        }

        //Metodos para mover la ventana
        [DllImport("User32.DLL", EntryPoint = "ReleaseCapture")] //importo las librerias necesarias para mover la ventana
        private extern static void ReleaseCapture(); //metodo para mover la ventana
        [DllImport("User32.DLL", EntryPoint = "SendMessage")] //importo las librerias necesarias para mover la ventana
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void ActualizarGrilla()
        {
            dgvTecnico.DataSource = null;
            dgvTecnico.DataSource = ControladoraTecnico.Instancia.RecuperarTecnicos();
            dgvTecnico.Columns["Tickets"].Visible = false;


            foreach (DataGridViewRow row in dgvTecnico.Rows)
            {
                var tecnico = (Tecnico)row.DataBoundItem;
                row.Cells["CantidadTickets"].Value = ControladoraTicket.Instancia.ContarTicketsPorTecnico(tecnico);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Form formTecnicoAM = new formTecnicoAM();
            formTecnicoAM.ShowDialog();
            ActualizarGrilla();
        }

        private void formTecnicoDGV_Load(object sender, EventArgs e)
        {
            ActualizarGrilla();

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTecnico.Rows.Count > 0)
            {
                var tecnicoSeleccionado = (Tecnico)dgvTecnico.CurrentRow.DataBoundItem;
                var confirmacion = MessageBox.Show("¿Está seguro que desea eliminar al técnico?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    var mensaje = ControladoraTecnico.Instancia.EliminarTecnico(tecnicoSeleccionado);
                    MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarGrilla();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un técnico para eliminarlo.");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvTecnico.Rows.Count > 0)
            {
                var tecnicoSeleccionado = (Tecnico)dgvTecnico.CurrentRow.DataBoundItem;
                formTecnicoAM formTecnicoAM = new formTecnicoAM(tecnicoSeleccionado);
                formTecnicoAM.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un tecnico para modificarlo.");
            }
            ActualizarGrilla();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtBuscarTecnico.Text != "Por nombre, DNI o legajo")
            {
                var listaTecnicos = ControladoraTecnico.Instancia.RecuperarTecnicos();
                var tecnicoEncontrado = listaTecnicos.FirstOrDefault(t => t.NombreyApellido.ToLower().Contains(txtBuscarTecnico.Text.ToLower()) || t.Dni.ToString().Contains(txtBuscarTecnico.Text) || t.Legajo.ToString().Contains(txtBuscarTecnico.Text));
                if (tecnicoEncontrado != null)
                {
                    dgvTecnico.DataSource = null;
                    dgvTecnico.DataSource = new List<Tecnico> { tecnicoEncontrado };
                }
                else
                {
                    MessageBox.Show("No se han encontrado los datos ingresados.");
                    ActualizarGrilla();
                }
            }
            else
            {
                MessageBox.Show("Ingrese un nombre y apellido, dni o legajo para buscar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarGrilla();
            }
        }

        private void txtBuscarTecnico_Enter(object sender, EventArgs e)
        {
            if (txtBuscarTecnico.Text == "Por nombre, DNI o legajo")
            {
                txtBuscarTecnico.Text = "";
                txtBuscarTecnico.ForeColor = Color.Black;
            }
        }

        private void txtBuscarTecnico_Leave(object sender, EventArgs e)
        {
            if (txtBuscarTecnico.Text == "")
            {
                txtBuscarTecnico.Text = "Por nombre, DNI o legajo";
                txtBuscarTecnico.ForeColor = Color.Silver;
            }
        }

        private void formTecnicoDGV_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void txtBuscarTecnico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressSoloLetras(e, txtBuscarTecnico.Text).Handled)
            {
                MessageBox.Show("Solo se permiten letras y números, no caracteres especiales", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
