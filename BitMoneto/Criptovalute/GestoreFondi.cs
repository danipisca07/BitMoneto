using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Criptovalute
{
    public class GestoreFondi
    {
        private List<IExchange> _exchanges;
        public List<IExchange> Exchanges { get { return _exchanges; } }

        private List<IBlockchain> _blockchains;
        public List<IBlockchain> Blockchains { get { return _blockchains; } }

        private ConcurrentDictionary<String, List<Fondo>> _fondi;
        public Dictionary<String, List<Fondo>> Fondi {
            get {
                return _fondi.ToDictionary(chiave=>chiave.Key, valore=>valore.Value);
            }
        }

        public GestoreFondi()
        {
            _fondi = new ConcurrentDictionary<string, List<Fondo>>();
            _exchanges = new List<IExchange>();
            _blockchains = new List<IBlockchain>();
        }


        #region Gestione Exchange
        /// <summary>
        /// Aggiunge un Exchange alla lista solo se non ce n'è già uno dello stesso tipo
        /// </summary>
        /// <param name="exchange">Exchange da aggiungere</param>
        /// <returns>True: Exchange aggiunto, False: Exchange già presente</returns>
        public bool AggiungiExchange(IExchange exchange)
        {
            //Controllo che negli exchange non ce ne sia già un altro dello stesso tipo
            if(_exchanges.All<IExchange>(elemento => elemento.GetType() != exchange.GetType()))
            {
                _exchanges.Add(exchange);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Rimuove un exchange dalla lista(trova l'exchange da eliminare cercandone uno del tipo indicato)
        /// </summary>
        /// <param name="tipoExchange">Type dell' exchange da eliminare</param>
        /// <returns>True: Exchange trovato e eliminato, False: non è stato trovato un exchange del tipo indicato</returns>
        public bool RimuoviExchange(Type tipoExchange)
        {
            if (_exchanges.Count == 0)
                return false;
            return _exchanges.RemoveAll((elem) => { return tipoExchange == elem.GetType(); }) > 0;

        }

        private async Task AggiornaExchanges()
        {
             var tasks = _exchanges.Select(async exchange =>
             {
                 List<Fondo> tmp = await exchange.ScaricaFondi();
                 if (!_fondi.TryAdd(exchange.Nome, tmp))
                     throw new Exception("Gestore fondi(AggiornaExchanges()): errore aggiunta fondi");
             });
             await Task.WhenAll(tasks);
        }
        #endregion

        #region Gestione Blockchain

        /// <summary>
        /// Aggiunge un nuovo monitor per un indirizzo sulla blockchain(controllando che non esista già per lo stesso indirizzo)
        /// </summary>
        /// <param name="blockchain">Elemento da aggiungere</param>
        /// <returns>True: Aggiunto, False: Elemento già presente</returns>
        public bool AggiungiBlockchain(IBlockchain blockchain)
        {
            //Controllo che non esista già un instanza della stessa blockchain
            if (_blockchains.All<IBlockchain>(elemento => elemento.GetType() != blockchain.GetType()))
            {
                _blockchains.Add(blockchain);
                return true;
            }
            else
                return false;
        }

        public bool RimuoviBlockchain(Type tipoBlockchain)
        {
            if (_blockchains.Count == 0)
                return false;
            return _blockchains.RemoveAll( (elem) => { return tipoBlockchain == elem.GetType(); }) > 0;
        }

        private async Task AggiornaBlockchains()
        {
            var tasks = _blockchains.Select(async blockchain =>
            {
                Portafoglio tmp = await blockchain.ScaricaPortafoglio();
                if (!_fondi.TryAdd(blockchain.Nome, new List<Fondo> { tmp.Fondo }))
                    throw new Exception("Gestore fondi(AggiornaBlockchains()):errore aggiunta fondi");
            });
            await Task.WhenAll(tasks);
        }
        #endregion

        public async Task AggiornaFondi()
        {
            _fondi = new ConcurrentDictionary<string, List<Fondo>>();
            Task aggiornamentoExchanges = AggiornaExchanges();
            Task aggiornamentoBlockchains = AggiornaBlockchains();
            await Task.WhenAll(aggiornamentoExchanges, aggiornamentoBlockchains);
        }

        

        
    }
}
