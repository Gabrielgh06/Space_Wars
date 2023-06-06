using ArenaDeBatala.GameLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace ArenaDeBatalha.GUI
{
    public partial class FormPrincipal : Form
    {
        DispatcherTimer gameLoopTimer { get; set; } // Relógio para o looping do jogo
        DispatcherTimer enemySpawnTimer { get; set; } // Relógio paara criação de inimigos
        Bitmap screenBuffer { get; set; } // Pintura de cada quadro do jogo
        Graphics screenPainter { get; set; } // Ferramenta de pintura
        Background background { get; set; } // Objeto plano de fundo
        Player player { get; set; } // Objeto Player
        GameOver gameOver { get; set; } // Objeto Game Over
        List<GameObject> gameObjects { get; set; } // Lista com todos os objetos renderizados na tela
        public Random random { get; set; } // Gerador de números aleatórios
        bool canShoot; // Variável que controla a quantidade de tiros qeu o jogador pode dar

        public FormPrincipal()
        {
            InitializeComponent(); // Inicializa todos os componentes

            this.random = new Random();
            this.ClientSize = Media.Background.Size; // Determina o tamanho da tela do formulário para que ela fique do mesmo tamanho da tela de fundo de background
            this.screenBuffer = new Bitmap(Media.Background.Width, Media.Background.Height);
            this.screenPainter = Graphics.FromImage(this.screenBuffer);
            this.gameObjects = new List<GameObject>();
            this.background = new Background(this.screenBuffer.Size, this.screenPainter);
            this.player = new Player(this.screenBuffer.Size, this.screenPainter);
            this.gameOver = new GameOver(this.screenBuffer.Size, this.screenPainter);

            this.gameLoopTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16.66666666666667);
            this.gameLoopTimer.Tick += GameLoop;

            this.enemySpawnTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.enemySpawnTimer.Interval = TimeSpan.FromMilliseconds(1000);
            this.enemySpawnTimer.Tick += SpawnEnemy; // Criação do inimigo

            StartGame(); // Inicializa o Jogo, que chama o Timer 
        }

        public void StartGame()
        {
            this.gameObjects.Clear();
            this.gameObjects.Add(background);
            this.gameObjects.Add(player);
            this.player.SetStartPosition();
            this.player.Active = true;
            this.gameLoopTimer.Start(); // O timer por sua vez é um looping do jogo
            this.enemySpawnTimer.Start();
            this. canShoot = true;
        }

        public void EndGame()
        {
            this.gameObjects.Clear();
            this.gameLoopTimer.Stop();
            this.enemySpawnTimer.Stop();
            this.background.UpdateObject();
            this.gameOver.UpdateObject();
            Invalidate();
        }

        public void SpawnEnemy(object sender, EventArgs e)
        {
            Point enemyPosition = new Point(this.random.Next(10,this.screenBuffer.Width - 74), -62);
            Enemy enemy = new Enemy(this.screenBuffer.Size, this.screenPainter, enemyPosition);
            this.gameObjects.Add(enemy);
        }

        public void GameLoop(object sender, EventArgs e) // O looíng percorre todos os objetos
        {
            this.gameObjects.RemoveAll(x => !x.Active);

            this.ProcessControls();

            foreach (GameObject go in this.gameObjects)
            {
                go.UpdateObject(); // e chama o método de atualização de todos os objetos
                                   // que foram colocados na coleção gameObject

                if (go.IsOutOfBounds())
                {
                    go.Destroy();
                }

                if (go is Enemy)
                {
                    if (go.IsCollidingWith(player))
                    {
                        player.Destroy();
                        player.PlaySound();
                        EndGame();
                        return;
                    }
                    foreach (GameObject bullet in this.gameObjects.Where(x => x is Bullet))
                    {
                        if (go.IsCollidingWith(bullet))
                        {
                            go.Destroy();
                            bullet.Destroy();
                        }
                    }
                }
            }
            this.Invalidate();
        }

        private void FormPrincipal_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Pintura do formulário pelo Buffer
            e.Graphics.DrawImage(this.screenBuffer, 0, 0);      // Que esta sendo alimentado pelos objetos
        }

        private void ProcessControls()
        {
            if (Keyboard.IsKeyDown(Key.Left)) player.MoveLeft();
            if (Keyboard.IsKeyDown(Key.Right)) player.MoveRight();
            if (Keyboard.IsKeyDown(Key.Up)) player.MoveUp();
            if (Keyboard.IsKeyDown(Key.Down)) player.MoveDown();

            if (Keyboard.IsKeyDown(Key.Space) && canShoot)
            {
                this.gameObjects.Insert(1,player.Shoot());
                this.canShoot = false;
            }
            if (Keyboard.IsKeyUp(Key.Space)) canShoot = true;
        }

        private void FormPrincipal_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                StartGame();
            }
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
