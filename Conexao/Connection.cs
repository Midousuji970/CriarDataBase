using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CriarDataBase.Conexao
{
   public class Connection
    {
       private string connectionString = "Server=127.0.0.1;Port=3306;uid=root;pwd=29814608;";
        public void ExisteBanco() // Faz a Verificação se existe o banco dentro do sistema
        {
            try {
                using (MySqlConnection conn = new MySqlConnection(connectionString)) { // Inicializa a Classe de Conexao ao BD
                using(MySqlCommand cmd = new MySqlCommand("show databases where database='tournstats'",conn)) // Inicializa a Classe que envia o comando sql
                    {
                        conn.Open(); // Inicializa a Conexao
                        Console.WriteLine("Conectado ao MariaDB");

                        using DbDataReader reader = cmd.ExecuteReader(); // Executa esse comando para ler a resposta do BD
                        if (reader.Read()) // Verifica se tem algum tipo de resposta que de para ler, se for nada ela não vira para ca
                        {                      
                            string saida = reader.GetString(0); // Como só quero o primeiro parametro passo para adquirir essa string, ja que a resposta é o nome do banco
                            if (saida.Contains("tournstats")) // Verifica se existe o nome do banco
                            {
                                Console.WriteLine("Banco Existe");
                                conn.Close();

                            }   
                        }
                        else
                        {
                            CriaBanco(); // Caso não receba nada, ele irá criar um banco novo
                        }
                    }
                }
            } catch {
                Console.WriteLine("Erro na Conexão");
            }

        }
        private void CriaBanco() // Cria o Banco caso não exista no MariaDB
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString)) //Inicializa a Classe de Conexão do C#, junto da string de conexão
                {

                    using (MySqlCommand cmd = new MySqlCommand("CREATE DATABASE tournstats CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci",conn)) // Inicializa a classe quem envia o comando SQL
                    {
                        conn.Open(); // Abre a Conexao
                        Console.WriteLine("Conectado ao banco");
                       
                        cmd.ExecuteNonQuery(); // Executa o comando no Banco de Dados
                        Console.WriteLine("Banco Criado com sucesso");
                        conn.Close(); // FEcha a Conexao
                        CriarTabelas();

                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro ao criar banco");
            }

           
        }

        private void CriarTabelas()
        {
            string[] tabelas = { "CREATE TABLE IF NOT EXISTS Times (id UUID PRIMARY KEY, nome VARCHAR(60) NOT NULL, dataCriado TIMESTAMP DEFAULT CURRENT_TIMESTAMP)",
                "CREATE TABLE IF NOT EXISTS Players (id UUID PRIMARY KEY, nome VARCHAR(60) NOT NULL, numero TINYINT UNSIGNED CHECK (numero BETWEEN 1 AND 99), teamId UUID, FOREIGN KEY (teamId) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS Characters (id INT PRIMARY KEY AUTO_INCREMENT, nome VARCHAR(60) NOT NULL, imagePath VARCHAR(255))",
            "CREATE TABLE IF NOT EXISTS Pets (id INT PRIMARY KEY AUTO_INCREMENT, nome VARCHAR(60) NOT NULL,imagePath VARCHAR(255))",
            "CREATE TABLE IF NOT EXISTS Attacks (id UUID PRIMARY KEY, playerId UUID NOT NULL, teamAdversario UUID NOT NULL, estrelas TINYINT UNSIGNED CHECK (estrelas BETWEEN 1 AND 5), dataAtaque TIMESTAMP DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (playerId) REFERENCES Players(id) ON DELETE CASCADE ON UPDATE CASCADE,FOREIGN KEY (teamAdversario) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS AttacksCharacters (attackId UUID NOT NULL, characterId INT NOT NULL, PRIMARY KEY (attackId, characterId), FOREIGN KEY (attackId) REFERENCES Attacks(id) ON DELETE CASCADE ON UPDATE CASCADE, FOREIGN KEY (characterId) REFERENCES Characters(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS AttacksPets (attackId UUID NOT NULL, petId INT NOT NULL, PRIMARY KEY (attackId, petId), FOREIGN KEY (attackId) REFERENCES Attacks(id) ON DELETE CASCADE ON UPDATE CASCADE, FOREIGN KEY (petId) REFERENCES Pets(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS Defenses (id UUID PRIMARY KEY, playerId UUID NOT NULL, dataDefendido TIMESTAMP DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (playerId) REFERENCES Players(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS DefenseCharacters (defenseId UUID NOT NULL,characterId INT NOT NULL, hide BOOLEAN DEFAULT FALSE,ban BOOLEAN DEFAULT FALSE, PRIMARY KEY (defenseId, characterId), FOREIGN KEY (defenseId) REFERENCES Defenses(id) ON DELETE CASCADE ON UPDATE CASCADE, FOREIGN KEY (characterId) REFERENCES Characters(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS DefensePets (defenseId UUID NOT NULL, petId INT NOT NULL, PRIMARY KEY (defenseId, petId), FOREIGN KEY (defenseId) REFERENCES Defenses(id) ON DELETE CASCADE ON UPDATE CASCADE, FOREIGN KEY (petId) REFERENCES Pets(id) ON DELETE CASCADE ON UPDATE CASCADE)",
            "CREATE TABLE IF NOT EXISTS Results (id INT PRIMARY KEY AUTO_INCREMENT, timeVencedor UUID NOT NULL, timePerdedor UUID NOT NULL, maxEstrelas INT CHECK (maxEstrelas >= 0), dataFinal TIMESTAMP DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (timeVencedor) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE,FOREIGN KEY (timePerdedor) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE)"
            };
            try { 
            using(MySqlConnection conn = new MySqlConnection(connectionString+ "Database=tournstats"))
                {
                    conn.Open();
                    foreach(string sql in tabelas)
                    {
                        string nomeTabela = sql.Split(' ')[5];
                        Console.WriteLine($"Criando Tabela: {nomeTabela}");
                        using(MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Tabela: {nomeTabela} Criada com Sucesso!");
                        }

                    }
                    conn.Close();
                }
            } catch { 
            Console.WriteLine("Erro ao criar Tabelas");
            }
        }
    }
    
}
