using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4emLinha
{
    public partial class Form1 : Form
    {
        const int Linhas = 6;
        const int Colunas = 7;
        Button[,] tabuleiro = new Button[Linhas, Colunas];
        int jogadorAtual = 1; // 1 ou 2
        float proporcao = 0.7f;
        Label[] labelsColunas = new Label[Colunas];

        Color playerOneColor = Color.Red;
        Color playerTwoColor = Color.Yellow;

        int playerOneScore = 0;
        int playerTwoScore = 0;

        bool firstClick = true;

        public Form1()
        {
            InitializeComponent();
            criarTabuleiro();
            configurarPlayers();
            panel1.Visible = false;
        }

        private void configurarPlayers()
        {
            button2.BackColor = playerOneColor;
            button3.BackColor = playerTwoColor;
        }

        private void criarTabuleiro()
        {
            int tamanhoCelula = 60;

            for (int linha = 0; linha < Linhas; linha++)
            {
                for (int coluna = 0; coluna < Colunas; coluna++)
                {
                    Button btn = new Button();
                    btn.Width = tamanhoCelula;
                    btn.Height = tamanhoCelula;
                    btn.Location = new Point(coluna * tamanhoCelula, linha * tamanhoCelula);
                    btn.Tag = new Point(linha, coluna); // Guarda a posição
                    btn.Click += Btn_Click;
                    btn.BackColor = Color.White;

                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;

                    // Torna o botão circular
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, btn.Width, btn.Height);
                    btn.Region = new Region(path);

                    tabuleiro[linha, coluna] = btn;
                    this.Controls.Add(btn);
                }

            }

            for (int i = 0; i < Colunas; i++)
            {
                var lbl = new Label();
                lbl.Text = (i + 1).ToString();
                lbl.TextAlign = ContentAlignment.TopCenter;
                lbl.AutoSize = false;
                lbl.ForeColor = Color.Black;
                lbl.Font = new Font("Arial", 10, FontStyle.Bold);
                this.Controls.Add(lbl);
                labelsColunas[i] = lbl;
            }

            RedesenharTabuleiro();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (firstClick) {
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                firstClick = false;
            }

            Button clicado = sender as Button;
            Point pos = (Point)clicado.Tag;

            int coluna = pos.Y;
            for (int linha = Linhas - 1; linha >= 0; linha--)
            {
                if (tabuleiro[linha, coluna].BackColor == Color.White)
                {
                    tabuleiro[linha, coluna].BackColor = jogadorAtual == 1 ? playerOneColor : playerTwoColor;
                    if (VerificaVitoria(linha, coluna))
                    {
                        DialogResult resultado = MessageBox.Show($"Jogador {jogadorAtual} venceu!");
                        DesabilitarTabuleiro();
                        if (jogadorAtual == 1) playerOneScore++; else playerTwoScore++;
                        AtualizarScore();
                        if (resultado == DialogResult.OK)
                        {
                            ResetTabuleiro();
                        }

                        
                    }
                    
                    if (checkBox2.Checked)
                    {
                        jogadorAtual = 2;

                        // Chama a jogada da CPU após pequena pausa
                        Task.Delay(300).ContinueWith(_ =>
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                JogadaCPU();
                            });
                        });

                    }
                    else { 
                        jogadorAtual = 3 - jogadorAtual; // Alterna entre 1 e 2
                    
                    }
                        break;
                }
            }
        }

        private bool VerificaVitoria(int linha, int coluna)
        {
            Color cor = tabuleiro[linha, coluna].BackColor;
            return VerificaDirecao(linha, coluna, 1, 0, cor)   // Vertical
                || VerificaDirecao(linha, coluna, 0, 1, cor)   // Horizontal
                || VerificaDirecao(linha, coluna, 1, 1, cor)   // Diagonal \
                || VerificaDirecao(linha, coluna, 1, -1, cor); // Diagonal /
        }

        private bool VerificaDirecao(int linha, int coluna, int dLinha, int dColuna, Color cor)
        {
            int cont = 1;

            // Verifica numa direção
            for (int i = 1; i < 4; i++)
            {
                int nl = linha + i * dLinha;
                int nc = coluna + i * dColuna;
                if (nl >= 0 && nl < Linhas && nc >= 0 && nc < Colunas &&
                    tabuleiro[nl, nc].BackColor == cor)
                {
                    cont++;
                }
                else break;
            }

            // Verifica na direção oposta
            for (int i = 1; i < 4; i++)
            {
                int nl = linha - i * dLinha;
                int nc = coluna - i * dColuna;
                if (nl >= 0 && nl < Linhas && nc >= 0 && nc < Colunas &&
                    tabuleiro[nl, nc].BackColor == cor)
                {
                    cont++;
                }
                else break;
            }

            return cont >= 4;
        }

        private void DesabilitarTabuleiro()
        {
            foreach (var btn in tabuleiro)
            {
                btn.Enabled = false;
            }
        }

        private void RedesenharTabuleiro()
        {

            int larguraDisponivel = (int)(this.ClientSize.Width * proporcao);
            int alturaDisponivel = (int)(this.ClientSize.Height * proporcao);

            // Tamanho da célula baseado na menor dimensão disponível
            int tamanhoCelula = Math.Min(larguraDisponivel / Colunas, alturaDisponivel / Linhas);

            // Tamanho real do tabuleiro (após arredondamento)
            int larguraTabuleiro = tamanhoCelula * Colunas;
            int alturaTabuleiro = tamanhoCelula * Linhas;

            // Centraliza o tabuleiro na tela
            int offsetX = (this.ClientSize.Width - larguraTabuleiro) / 2;
            int offsetY = 4* (this.ClientSize.Height - alturaTabuleiro) / 5; 

            for (int linha = 0; linha < Linhas; linha++)
            {
                for (int coluna = 0; coluna < Colunas; coluna++)
                {
                    Button btn = tabuleiro[linha, coluna];
                    btn.Width = tamanhoCelula;
                    btn.Height = tamanhoCelula;
                    btn.Location = new Point(
                        offsetX + coluna * tamanhoCelula,
                        offsetY + linha * tamanhoCelula
                    );

                    // Atualiza formato redondo
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, btn.Width, btn.Height);
                    btn.Region = new Region(path);
                }
            }

            for (int coluna = 0; coluna < Colunas; coluna++)
            {
                Label lbl = labelsColunas[coluna];
                lbl.Width = tamanhoCelula;
                lbl.Height = 20; // altura do texto
                lbl.Location = new Point(
                    offsetX + coluna * tamanhoCelula,
                    offsetY + Linhas * tamanhoCelula + 5 // logo abaixo da última linha
                );
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            RedesenharTabuleiro();
        }

        private void ResetTabuleiro()
        {
            
            jogadorAtual = 1;

            foreach (var btn in tabuleiro)
            {
                btn.BackColor = Color.White;
                btn.Enabled = true;  
            }

            foreach (var lbl in labelsColunas)
            {
                lbl.Text = (Array.IndexOf(labelsColunas, lbl) + 1).ToString(); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playerOneScore = 0;
            playerTwoScore = 0;
            AtualizarScore();
            ResetTabuleiro();
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;
            firstClick = true;
        }

        private void AtualizarScore()
        {
            scoreOne.Text = playerOneScore.ToString();
            scoreTwo.Text = playerTwoScore.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                checkBox2.Checked = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                checkBox1.Checked = false;
        }


        private void JogadaCPU()
        {
            List<int> colunasValidas = new List<int>();

            // Verifica colunas disponíveis (com espaço livre na linha inferior)
            for (int col = 0; col < Colunas; col++)
            {
                if (tabuleiro[0, col].BackColor == Color.White) // se a célula do topo estiver branca, a coluna ainda tem espaço
                    colunasValidas.Add(col);
            }

            if (colunasValidas.Count == 0)
                return; // Empate, nenhum espaço

            // Escolhe coluna aleatória entre as válidas
            Random rand = new Random();
            int colunaEscolhida = colunasValidas[rand.Next(colunasValidas.Count)];

            // Simula o clique na coluna escolhida (de cima para baixo)
            for (int linha = Linhas - 1; linha >= 0; linha--)
            {
                if (tabuleiro[linha, colunaEscolhida].BackColor == Color.White)
                {
                    tabuleiro[linha, colunaEscolhida].BackColor = Color.Yellow;

                    if (VerificaVitoria(linha, colunaEscolhida))
                    {
                        MessageBox.Show("CPU venceu!");
                        DesabilitarTabuleiro();
                    }

                    jogadorAtual = 1;
                    return;
                }
            }
        }

        
    }
}
