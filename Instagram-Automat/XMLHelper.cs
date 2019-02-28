using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Instagram_Automat
{
	class XMLHelper
	{
		private XmlTextWriter _writer;
		private XmlTextReader _reader;
		private string _path = "C:\\myXmFile.xml";
		private XmlNodeList _nodos;
		private XmlDocument _file;
		private string _userName;

		public XMLHelper(string userName)
		{
			//_writer = new XmlTextWriter(_path, null);
			//_reader = new XmlTextReader(_path);

			_userName = userName; 
			_file = new XmlDocument();
			_file.Load(_path);
			
			
		}

		public void EscribirSeguidos()
		{
			
		}

		public int Seguidores()
		{
			return Convert.ToInt32(_file.SelectSingleNode($"/{_userName}/seguidores")?.InnerText);
		}

		public int Seguidos()
		{
			return Convert.ToInt32(_file.SelectSingleNode($"/{_userName}/seguidos")?.InnerText);
		}
	}
}
