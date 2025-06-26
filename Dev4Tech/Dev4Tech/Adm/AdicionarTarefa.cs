﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Dev4Tech
{
    public partial class AdicionarTarefa : Form
    {
        private string caminhoArquivoSelecionado = "";
        private List<int> equipesSelecionadas = new List<int>(); // Lista interna para armazenar equipes selecionadas

        public AdicionarTarefa()
        {
            InitializeComponent();

            // Carrega equipes no ComboBox
            CarregarEquipes();

            // Configura eventos dos botões
            btnAnexarArquivos.Click += BtnAnexarArquivos_Click;
            btnAddTarefas.Click += BtnAddTarefas_Click;
            btnAddEquipe.Click += BtnAddEquipe_Click;

            // Inicializa comboBox de dificuldade
            cmbDificuldade.Items.AddRange(new string[] { "Fácil", "Média", "Difícil" });
            cmbDificuldade.SelectedIndex = 1; // Seleciona "Média" por padrão
        }

        // Busca equipes do banco e carrega no ComboBox
        private void CarregarEquipes()
        {
            AddTarefas tarefa = new AddTarefas();
            DataTable dt = tarefa.BuscarEquipes();

            cmbAddEquipe.DataSource = dt;
            cmbAddEquipe.DisplayMember = "nome_equipe";
            cmbAddEquipe.ValueMember = "id_equipe";
            cmbAddEquipe.SelectedIndex = -1;
        }

        // Evento para anexar arquivo
        private void BtnAnexarArquivos_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Todos os arquivos (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                caminhoArquivoSelecionado = ofd.FileName;
                lblArquivosSelecionado.Text = Path.GetFileName(caminhoArquivoSelecionado);
            }
        }

        // Evento para adicionar equipe selecionada à lista interna
        private void BtnAddEquipe_Click(object sender, EventArgs e)
        {
            if (cmbAddEquipe.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione uma equipe para adicionar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idEquipe = Convert.ToInt32(cmbAddEquipe.SelectedValue);

            if (!equipesSelecionadas.Contains(idEquipe))
            {
                equipesSelecionadas.Add(idEquipe);
                MessageBox.Show("Equipe selecionada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Essa equipe já foi selecionada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Evento para adicionar tarefa no banco para todas as equipes selecionadas
        private void BtnAddTarefas_Click(object sender, EventArgs e)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(txtInstruções.Text))
            {
                MessageBox.Show("Por favor, preencha as instruções da tarefa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNomeTarefa.Text))
            {
                MessageBox.Show("Por favor, preencha o nome da tarefa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (equipesSelecionadas.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos uma equipe.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpDataDeEntrega.Value.Date < DateTime.Today)
            {
                MessageBox.Show("A data de entrega deve ser hoje ou uma data futura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbDificuldade.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione a dificuldade da tarefa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Dados coletados do formulário
            string nomeTarefa = txtNomeTarefa.Text.Trim();
            string instrucoes = txtInstruções.Text.Trim();
            string dificuldade = cmbDificuldade.SelectedItem.ToString();
            DateTime dataEntrega = dtpDataDeEntrega.Value.Date;
            byte[] arquivoBytes = null;
            string nomeArquivo = "";

            // Lê arquivo se selecionado
            if (!string.IsNullOrEmpty(caminhoArquivoSelecionado))
            {
                try
                {
                    arquivoBytes = File.ReadAllBytes(caminhoArquivoSelecionado);
                    nomeArquivo = Path.GetFileName(caminhoArquivoSelecionado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao ler o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Insere tarefa para cada equipe selecionada
            foreach (int idEquipe in equipesSelecionadas)
            {
                AddTarefas tarefa = new AddTarefas
                {
                    NomeTarefa = nomeTarefa,
                    Instrucoes = instrucoes,
                    Dificuldade = dificuldade,
                    IdEquipe = idEquipe,
                    DataEntrega = dataEntrega,
                    NomeArquivo = nomeArquivo,
                    ArquivoBlob = arquivoBytes
                };

                try
                {
                    tarefa.Inserir();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao adicionar tarefa para equipe ID {idEquipe}: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            MessageBox.Show("Tarefas adicionadas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimparFormulario();
        }

        // Limpa campos após inserção
        private void LimparFormulario()
        {
            txtInstruções.Clear();
            txtNomeTarefa.Clear();
            equipesSelecionadas.Clear();
            cmbAddEquipe.SelectedIndex = -1;
            cmbDificuldade.SelectedIndex = 1;
            dtpDataDeEntrega.Value = DateTime.Today;
            caminhoArquivoSelecionado = "";
            lblArquivosSelecionado.Text = "Nenhum arquivo selecionado";
        }

        // Eventos mantidos
        private void btnHome_Click(object sender, EventArgs e)
        {
            Home h = new Home();
            h.Show();
            this.Hide();
        }

        private void btnEquipes_Click(object sender, EventArgs e)
        {
            Equipes_Estatisticas equip_e = new Equipes_Estatisticas();
            equip_e.Show();
            this.Hide();
        }

        private void btnRanking_Click(object sender, EventArgs e)
        {
            Ranking_Equipes rk = new Ranking_Equipes();
            rk.Show();
            this.Hide();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var funcionario = Sessao.FuncionarioLogado;

            if (funcionario != null)
            {
                Configuracoes config = new Configuracoes(funcionario);
                config.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nenhum funcionário logado.");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void btnAddTarefas_Click_1(object sender, EventArgs e)
        {
            // Pode deixar vazio ou implementar o que for necessário
        }

        private void txtNomeTarefa_TextChanged(object sender, EventArgs e)
        {
            // Pode deixar vazio ou implementar o que for necessário
        }

        private void AdicionarTarefa_Load(object sender, EventArgs e)
        {

        }

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            Tarefas_Completadas t_completadas = new Tarefas_Completadas();
            t_completadas.Show();
            this.Hide();
        }
    }
}
