using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BoletoNet;
using BoletoNet.Arquivo.Class;

namespace BoletoNet.Arquivo
{
    public partial class Main : Form
    {
        #region ImpressaoBoleto
        private short _codigoBanco = 0;
        private Progresso _progresso;
        string _arquivo = string.Empty;
        private ImpressaoBoleto _impressaoBoleto = new ImpressaoBoleto();

        public short CodigoBanco
        {
            get { return _codigoBanco; }
            set { _codigoBanco = value; }
        }

        void _impressaoBoleto_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        #endregion

        #region GERA LAYOUT DO BOLETO
        private void GeraLayout(List<BoletoBancario> boletos)
        {
            try
            {
                StringBuilder html = new StringBuilder();
                foreach (BoletoBancario o in boletos)
                {
                    html.Append(o.MontaHtml());
                    html.Append("</br></br></br></br></br></br></br></br></br></br>");
                }

                _arquivo = System.IO.Path.GetTempFileName();

                using (FileStream f = new FileStream(_arquivo, FileMode.Create))
                {
                    StreamWriter w = new StreamWriter(f, System.Text.Encoding.Default);
                    w.Write(html.ToString());
                    w.Close();
                    f.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion

        #region Bancos

        #region BOLETO Caixa
        private void GeraBoletoCaixa(int qtde)
        {
            try
            {
                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {
                    #region Classes
                    CedenteDTO dtoced = new CedenteDTO();
                    dtoced.nome = NOMEtextBox.Text;
                    dtoced.cpfcnpj = CPFtextBox.Text;
                    dtoced.agencia = AGtextBox.Text;
                    dtoced.conta = CCtextBox.Text;

                    SacadoDTO dtosac = new SacadoDTO();
                    dtosac.cpfcnpj = cpfsactextBox.Text;
                    dtosac.nome = nomesactextBox.Text;

                    EnderecoDTO dtoend = new EnderecoDTO();
                    dtoend.End = endtextBox.Text;
                    dtoend.Bairro = bairrotextBox.Text;
                    dtoend.Cidade = CidadetextBox.Text;
                    dtoend.CEP = CEPtextBox.Text;
                    dtoend.UF = UFtextBox.Text;

                    //Instrucao_Caixa item1 = new Instrucao_Caixa(9, 5);
                    //Instrucao_Caixa item2 = new Instrucao_Caixa(81, 10);

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    
                    string NossoNumero = NossoNumerotextBox.Text;
                    string NumeroDocumento = NumeroDocumentotextBox.Text;

                    #endregion

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;


                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);

                    #region Vencimento


                        DateTime vencimento = DateTime.Now;
                        string datatextbox = VencimentoTextbox.Text.Replace(" ", "").Replace("/", "");
                        DateTime vencimentodotextbox = Convert.ToDateTime(datatextbox);
                        vencimento = vencimentodotextbox;
                        DateTime _dia = DateTime.Now;
                        DateTime vencimentoem5dias = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                   
                    #endregion

                    Boleto b = new Boleto(vencimento, valorboleto, "SR", NossoNumero, c);
                    b.NumeroDocumento = NumeroDocumento;

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    string instrucoes = instrucoestextBox.Text.ToUpper();

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = instrucoes;
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        #endregion

        #region BOLETO ITAÚ
        private void GeraBoletoItau(int qtde)
        {
            try
            {
                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {
                    CedenteDTO dtoced = new CedenteDTO();
                    dtoced.nome = NOMEtextBox.Text;
                    dtoced.cpfcnpj = CPFtextBox.Text;
                    dtoced.agencia = AGtextBox.Text;
                    dtoced.conta = CCtextBox.Text;

                    SacadoDTO dtosac = new SacadoDTO();
                    dtosac.cpfcnpj = cpfsactextBox.Text;
                    dtosac.nome = nomesactextBox.Text;

                    EnderecoDTO dtoend = new EnderecoDTO();
                    dtoend.End = endtextBox.Text;
                    dtoend.Bairro = bairrotextBox.Text;
                    dtoend.Cidade = CidadetextBox.Text;
                    dtoend.CEP = CEPtextBox.Text;
                    dtoend.UF = UFtextBox.Text;

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;

                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    //Na carteira 198 o código do Cedente é a conta bancária
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());

                    string carteira = "198";
                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);

                    Boleto b = new Boleto(vencimento, valorboleto, carteira, "00000865425", c, new EspecieDocumento(341, 1));
                    b.NumeroDocumento = "000032548";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO SUDAMERIS
        public void GeraBoletoSudameris(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;


                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());

                    //Nosso número com 7 dígitos
                    string nn = "0003020";
                    //Nosso número com 13 dígitos
                    //nn = "0000000003025";

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);

                    Boleto b = new Boleto(vencimento, valorboleto, "198", nn, c);// EnumEspecieDocumento_Sudameris.DuplicataMercantil);
                    b.NumeroDocumento = "1008073";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO SAFRA
        public void GeraBoletoSafra(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;

                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    Boleto b = new Boleto(vencimento, valorboleto, "198", "02592082835", c);
                    b.NumeroDocumento = "1008073";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();
                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO REAL
        public void GeraBoletoReal(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;


                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    Boleto b = new Boleto(vencimento, valorboleto, "57", "92082835", c);
                    b.NumeroDocumento = "1008073";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO HSBC
        public void GeraBoletoHsbc(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;

                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    // Código fornecido pela agencia, NÃO é o numero da conta
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());// 7 posicoes                

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    Boleto b = new Boleto(vencimento, valorboleto, "CNR", "0000321585", c); //cod documento
                    b.NumeroDocumento = "0000032548956"; // nr documento

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO BANCO DO BRASIL
        public void GeraBoletoBB(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;

                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());

                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    Boleto b = new Boleto(vencimento, valorboleto, "18", "12345678901", c);
                    b.NumeroDocumento = "12345678901";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #region BOLETO BRADESCO
        public void GeraBoletoBradesco(int qtde)
        {
            try
            {
                CedenteDTO dtoced = new CedenteDTO();
                dtoced.nome = NOMEtextBox.Text;
                dtoced.cpfcnpj = CPFtextBox.Text;
                dtoced.agencia = AGtextBox.Text;
                dtoced.conta = CCtextBox.Text;

                SacadoDTO dtosac = new SacadoDTO();
                dtosac.cpfcnpj = cpfsactextBox.Text;
                dtosac.nome = nomesactextBox.Text;

                EnderecoDTO dtoend = new EnderecoDTO();
                dtoend.End = endtextBox.Text;
                dtoend.Bairro = bairrotextBox.Text;
                dtoend.Cidade = CidadetextBox.Text;
                dtoend.CEP = CEPtextBox.Text;
                dtoend.UF = UFtextBox.Text;

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();
                for (int i = 0; i < qtde; i++)
                {

                    bb = new BoletoBancario();
                    bb.CodigoBanco = _codigoBanco;

                    DateTime _dia = DateTime.Now;
                    DateTime vencimento = Convert.ToDateTime(_dia.AddDays(5).ToString("dd/MM/yyyy"));

                    Cedente c = new Cedente(dtoced.cpfcnpj, dtoced.nome, dtoced.agencia, dtoced.conta);
                    c.Codigo = Convert.ToInt32(dtoced.conta.ToString());




                    double valorboleto = Convert.ToDouble(ValorBoletotextBox.Text);
                    Boleto b = new Boleto(vencimento, valorboleto, "02", "01000000001", c);
                    b.NumeroDocumento = "01000015235";

                    b.Sacado = new Sacado(dtosac.cpfcnpj, dtosac.nome);
                    b.Sacado.Endereco.End = dtoend.End;
                    b.Sacado.Endereco.Bairro = dtoend.Bairro;
                    b.Sacado.Endereco.Cidade = dtoend.Cidade;
                    b.Sacado.Endereco.CEP = dtoend.CEP;
                    b.Sacado.Endereco.UF = dtoend.UF;

                    Instrucao instr = new Instrucao(001);
                    instr.Descricao = "Não Receber após o vencimento";
                    b.Instrucoes.Add(instr);

                    bb.Boleto = b;
                    bb.Boleto.Valida();

                    boletos.Add(bb);
                }

                GeraLayout(boletos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }
        #endregion

        #endregion

        #region Eventos do BackgroundWorker
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (CodigoBanco)
                {
                    case 1: // Banco do Brasil
                        GeraBoletoBB(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 347: // Sudameris
                        GeraBoletoSudameris(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 422: // Safra
                        GeraBoletoSafra(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 341: // Itau
                        GeraBoletoItau(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 356: // Real
                        GeraBoletoReal(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 399: // Hsbc
                        GeraBoletoItau(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 237: // Bradesco
                        GeraBoletoBradesco(Convert.ToInt32(numericUpDown.Value));
                        break;

                    case 104: // Caixa
                        GeraBoletoCaixa(Convert.ToInt32(numericUpDown.Value));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alerta de Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _progresso.Close();

                // Cria um formulário com um componente WebBrowser dentro
                _impressaoBoleto.webBrowser.Navigate(_arquivo);
                _impressaoBoleto.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            

        }
        #endregion Eventos do BackgroundWorker

        #region Load, Button Chamar Bancos
        public Main()
        {
            InitializeComponent();

            _impressaoBoleto.FormClosed += new FormClosedEventHandler(_impressaoBoleto_FormClosed);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ChamarBanco();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alerta de Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ChamarBanco()
        {
            try
            {
                if (radioButtonItau.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonItau.Tag);
                else if (radioButtonSudameris.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonSudameris.Tag);
                else if (radioButtonSafra.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonSafra.Tag);
                else if (radioButtonReal.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonReal.Tag);
                else if (radioButtonHsbc.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonHsbc.Tag);
                else if (radioButtonBancoBrasil.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonBancoBrasil.Tag);
                else if (radioButtonBradesco.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonBradesco.Tag);
                else if (radioButtonCaixa.Checked)
                    CodigoBanco = Convert.ToInt16(radioButtonCaixa.Tag);

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                backgroundWorker.RunWorkerAsync();
                _progresso = new Progresso();
                _progresso.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message , "Alerta de Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        #endregion



    }
}