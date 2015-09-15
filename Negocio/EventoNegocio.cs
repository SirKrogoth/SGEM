﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AcessoBancoDados;
using ObjetoTransferencia;
using System.Data.SqlClient;
using System.Data;

namespace Negocio
{
    public class EventoNegocio
    {
        AcessaDadosSqlServer acessaDados = new AcessaDadosSqlServer();

        public string InserirEvento(Evento evento)
        {
            try
            {
                acessaDados.limparParametro();
                acessaDados.adicionarParametro("@nome", evento.nome);
                acessaDados.adicionarParametro("@localEvento", evento.localEvento);
                acessaDados.adicionarParametro("@data_evento", evento.data_evento);
                acessaDados.adicionarParametro("@inicio", evento.inicio);
                acessaDados.adicionarParametro("@termino", evento.termino);
                acessaDados.adicionarParametro("@tema", evento.tema);
                acessaDados.adicionarParametro("@codCliente", evento.codCliente);
                acessaDados.adicionarParametro("@observacao", evento.observacao);
                acessaDados.adicionarParametro("@totalEvento", evento.totalEvento);
                acessaDados.adicionarParametro("@codParametro", evento.parametro);
                acessaDados.adicionarParametro("@concluido", 0);
                acessaDados.adicionarParametro("@cancelado", 0);
                acessaDados.adicionarParametro("@cidadeEvento", evento.cidadeEvento);

                string codEvento = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_INSERIR_EVENTO").ToString();

                return codEvento;
            }
            catch(Exception e)
            {
                return e.Message;
            }          
        }

        public string AlterarEvento(Evento evento)
        {
            try
            {
                acessaDados.limparParametro();
                acessaDados.adicionarParametro("@nome", evento.nome);
                acessaDados.adicionarParametro("@local", evento.localEvento);
                acessaDados.adicionarParametro("@data_evento", evento.data_evento);
                acessaDados.adicionarParametro("@inicio", evento.inicio);
                acessaDados.adicionarParametro("@termino", evento.termino);
                acessaDados.adicionarParametro("@tema", evento.tema);
                acessaDados.adicionarParametro("@codCliente", evento.codCliente);
                acessaDados.adicionarParametro("@referencia", evento.observacao);
                acessaDados.adicionarParametro("@codEvento", evento.codEvento);

                string codEvento = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_ALTERAR_EVENTO").ToString();

                return codEvento;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public string ExcluirEvento(Evento evento)
        {
            try
            {
                acessaDados.limparParametro();
                acessaDados.adicionarParametro("@codEvento", evento.codEvento);

                string codEvento = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_EXCLUIR_EVENTO").ToString();

                return codEvento;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public EventoColecao ConsultarNomeEvento(string nomeEvento)
        {
            try
            {
                EventoColecao ec = new EventoColecao();

                acessaDados.limparParametro();
                acessaDados.adicionarParametro("@nome", nomeEvento);

                DataTable retornoBanco = acessaDados.executarConsulta(CommandType.StoredProcedure, "SP_CONSULTAR_NOME_EVENTO");

                foreach(DataRow linha in retornoBanco.Rows)
                {
                    Evento evento = new Evento();

                    evento.codEvento = Convert.ToInt32(linha["codEvento"]);
                    evento.nome = Convert.ToString(linha["nome"]);
                    evento.localEvento = Convert.ToString(linha["localEvento"]);
                    evento.data_evento = Convert.ToDateTime(linha["data_evento"]);
                    evento.inicio = (TimeSpan)linha["inicio"];
                    evento.termino = (TimeSpan)linha["termino"];
                    evento.tema = Convert.ToString(linha["tema"]);
                    evento.codCliente = Convert.ToInt32(linha["codCliente"]);
                    evento.observacao = Convert.ToString(linha["observacao"]);

                    ec.Add(evento);
                }

                return ec;
            }
            catch(Exception e)
            {
                throw new Exception("Não foi possível consultar dados. Detalhes: " + e.Message);
            }
        }
        //Evento independente onde será retornado todos os eventos a serem realizados.
        public EventoColecao ConsultaProximosEventos()
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                //Abrindo conexao com o banco de dados 
                conexao.Open();

                EventoColecao eventoColecao = new EventoColecao();

                DateTime hoje = new DateTime();
                hoje = DateTime.Now;

                string script = "SELECT e.codEvento, e.nome, e.localEvento, e.data_evento, e.inicio, e.termino, e.tema, e.codCliente, e.observacao, e.concluido, e.cancelado, e.cidadeEvento, c.nome AS nomeCliente" +
                    " FROM tblEvento AS e" +
                    " INNER JOIN tblCliente AS c" +
                    " ON e.codCliente = c.codCliente" +
                    " WHERE data_evento >= '" + hoje +
                    "' AND concluido = 0" +
                    " AND cancelado = 0" +
                    " ORDER BY data_evento ASC";

                SqlCommand sql = new SqlCommand(script, conexao);
                //Irá retornar as informações do banco de dados
                SqlDataReader dataReader = sql.ExecuteReader();
                DataTable dataTable = new DataTable();
                //colocando os dados do dataReader dentro de um DataTable
                dataTable.Load(dataReader);
                
                foreach (DataRow linha in dataTable.Rows)
                {
                    Evento evento = new Evento();

                    evento.codEvento = Convert.ToInt32(linha["codEvento"]);
                    evento.nome = Convert.ToString(linha["nome"]);
                    evento.localEvento = Convert.ToString(linha["localEvento"]);
                    evento.data_evento = Convert.ToDateTime(linha["data_evento"]);
                    evento.inicio = (TimeSpan)linha["inicio"];
                    evento.termino = (TimeSpan)linha["termino"];
                    evento.tema = Convert.ToString(linha["tema"]);
                    evento.codCliente = Convert.ToInt32(linha["codCliente"]);
                    evento.observacao = Convert.ToString(linha["observacao"]);
                    evento.concluido = Convert.ToBoolean(linha["concluido"]);
                    evento.cancelado = Convert.ToBoolean(linha["cancelado"]);
                    evento.cidadeEvento = linha["cidadeEvento"].ToString();
                    evento.nomeCliente = linha["nomeCliente"].ToString();

                    eventoColecao.Add(evento);
                }

                return eventoColecao;
            }
            catch(Exception e)
            {
                throw new Exception("Não foi possível consultar dados. Detalhes: " + e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }
        //Evento independente onde será retornado todos os eventos a serem realizados de forma avançada.
        public EventoColecao ConsultaAvancada(string cliente, string aniversariante, string cidade, DateTime dataDe, DateTime dataPara, bool concluido, bool cancelado)
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                conexao.Open();
                EventoColecao eventoColecao = new EventoColecao();
                string script = "SELECT e.codEvento, e.nome, e.localEvento, e.data_evento, e.inicio, e.termino, e.tema, e.codCliente, e.observacao, e.concluido, e.cancelado, e.cidadeEvento, c.nome AS nomeCliente" + 
                    " FROM tblEvento AS e" +
                    " INNER JOIN tblCliente AS c " +
                    " ON e.codCliente = c.codCliente " +
                    " WHERE c.nome LIKE '%" + cliente + "%' AND e.nome LIKE '%" + aniversariante + "%' AND e.cidadeEvento LIKE '%" + cidade + "%' AND e.data_evento >= '" + dataDe + "' AND e.data_evento <= '" + dataPara + "' AND (e.concluido = '" + concluido + "' OR e.concluido = 'false') AND (e.cancelado = '" + cancelado + "' OR e.cancelado = 'false' )ORDER BY data_evento ASC";
                SqlCommand sql = new SqlCommand(script, conexao);
                SqlDataReader dataReader = sql.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                foreach (DataRow linha in dataTable.Rows)
                {
                    Evento evento = new Evento();

                    evento.codEvento = Convert.ToInt32(linha["codEvento"]);
                    evento.nome = linha["nome"].ToString();
                    evento.localEvento = Convert.ToString(linha["localEvento"]);
                    evento.data_evento = Convert.ToDateTime(linha["data_evento"]);
                    evento.inicio = (TimeSpan)linha["inicio"];
                    evento.termino = (TimeSpan)linha["termino"];
                    evento.tema = Convert.ToString(linha["tema"]);
                    evento.codCliente = Convert.ToInt32(linha["codCliente"]);
                    evento.observacao = Convert.ToString(linha["observacao"]);
                    evento.concluido = Convert.ToBoolean(linha["concluido"]);
                    evento.cancelado = Convert.ToBoolean(linha["cancelado"]);
                    evento.cidadeEvento = linha["cidadeEvento"].ToString();
                    evento.nomeCliente = linha["nomeCliente"].ToString();

                    eventoColecao.Add(evento);
                }

                return eventoColecao;

            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }

        public string InserirEventoBrinquedo(EventoBrinquedoColecao eventoBrinquedoColecao)
        {
            try
            {
                string codEventoBrinquedo = "0";

                for(int i = 0; i < eventoBrinquedoColecao.Count; i++)
                {
                    acessaDados.limparParametro();
                    acessaDados.adicionarParametro("@codEvento", eventoBrinquedoColecao[i].codEvento);
                    acessaDados.adicionarParametro("@codBrinquedo", eventoBrinquedoColecao[i].codBrinquedo);
                    //irá retornar o codigo do evento
                    codEventoBrinquedo = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_INSERIR_EVENTOBRINQUEDO").ToString();
                }
                
                return codEventoBrinquedo;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public string InserirEventoDecoracao(EventoDecoracaoColecao eventoDecoracaoColecao)
        {
            try
            {
                string codEventoDecoracao = "0";

                for(int i = 0; i < eventoDecoracaoColecao.Count; i++)
                {
                    acessaDados.limparParametro();
                    acessaDados.adicionarParametro("@codEvento", eventoDecoracaoColecao[i].codEvento);
                    acessaDados.adicionarParametro("@codDecoracao", eventoDecoracaoColecao[i].codDecoracao);

                    codEventoDecoracao = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_INSERIR_EVENTODECORACAO").ToString();

                }

                return codEventoDecoracao;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public string InserirEventoServico(EventoServicoColecao eventoServicoColecao)
        {
            try
            {
                string codEventoServico = "0";

                for(int i = 0; i < eventoServicoColecao.Count; i++)
                {
                    acessaDados.limparParametro();
                    acessaDados.adicionarParametro("@codServico", eventoServicoColecao[i].codServico);
                    acessaDados.adicionarParametro("@codEvento", eventoServicoColecao[i].codEvento);

                    codEventoServico = acessaDados.executarManipulacao(CommandType.StoredProcedure, "SP_INSERIR_EVENTOSERVICO").ToString();

                }

                return codEventoServico;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public void EventoConcluir(Evento evento)
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                conexao.Open();
                string sql = "UPDATE tblEvento SET concluido = 1 WHERE codEvento = " + evento.codEvento;
                SqlCommand cmd = new SqlCommand(sql, conexao);
                cmd.ExecuteNonQuery();                
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }

        public void EventoConcluirDesfazer(Evento evento)
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                conexao.Open();
                string sql = "UPDATE tblEvento SET concluido = 0 WHERE codEvento = " + evento.codEvento;
                SqlCommand cmd = new SqlCommand(sql, conexao);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }

        //métodos para cancelar evento
        public void EventoCancelado(Evento evento)
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                conexao.Open();
                string sql = "UPDATE tblEvento SET cancelado = 1 WHERE codEvento = " + evento.codEvento;
                SqlCommand cmd = new SqlCommand(sql, conexao);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }

        public void EventoCanceladoDesfazer(Evento evento)
        {
            SqlConnection conexao = acessaDados.criarConexaoBanco();
            try
            {
                conexao.Open();
                string sql = "UPDATE tblEvento SET cancelado = 0 WHERE codEvento = " + evento.codEvento;
                SqlCommand cmd = new SqlCommand(sql, conexao);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conexao.Close();
            }
        }
    }
}