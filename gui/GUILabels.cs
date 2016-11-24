/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace gui
{
    /// <summary>
    /// Spanish translation by Di3go
    /// </summary>
    class GUILabels
    {
        public static Dictionary<String, String> english = new Dictionary<String, String>();
        public static Dictionary<String, String> spanish = new Dictionary<String, String>();

        public static bool IsSpanish { get; set; }

        public static void Setup(MainWindow w)
        {
            english["button1a"] = "Start server";
            spanish["button1a"] = "Comenzar el servidor";

            english["button1b"] = "Stop server";
            spanish["button1b"] = "Detener el servidor";

            english["statuslabela"] = "Status: Server stopped.";
            spanish["statuslabela"] = "Estado: Servidor detenido.";

            english["statuslabelb"] = "Status: Server running.";
            spanish["statuslabelb"] = "Estado: Servidor funcionando";

            english["mboxa"] = "Unable to start server - please check your settings";
            spanish["mboxa"] = "No se puede iniciar el servidor - Por favor revise su configuración";

            english["mboxb"] = "Are you sure you want to quit sb0t?";
            spanish["mboxb"] = "Estás seguro que deseas salir de Sb0t?";

            english["mboxc"] = "Invalid leaf identifier";
            spanish["mboxc"] = "Identificador de hoja invalido";

            english["mboxd"] = "This leaf already exists in your trusted leaf list";
            spanish["mboxd"] = "Está hoja ya existe en su lista de hojas de confianza";

            english["mboxe"] = "Invalid sb0t extension";
            spanish["mboxe"] = "Extensión de sb0t invalida";

            english["label2"] = "Room name:";
            spanish["label2"] = "Nombre de la sala:";

            english["label3"] = "Room port:";
            spanish["label3"] = "Puerto de la sala:";

            english["label4"] = "Bot name:";
            spanish["label4"] = "Nombre del bot:";

            english["checkBox1"] = "Chat logging enabled";
            spanish["checkBox1"] = "Registro de Chat habilitado";

            english["checkBox2"] = "Room scribbles enabled";
            spanish["checkBox2"] = "Scribbles en sala habilitados";

            english["checkBox3"] = "Start server when sb0t starts";
            spanish["checkBox3"] = "Iniciar servidor cuando sb0t inicie";

            english["checkBox4"] = "Load sb0t when windows starts";
            spanish["checkBox4"] = "Cargar sb0t cuando windows comience";

            english["checkBox5"] = "Show room on channel list";
            spanish["checkBox5"] = "Mostrar la sala en la lista de canales";

            english["checkBox6"] = "Support voice chat";
            spanish["checkBox6"] = "Soporte de chat de voz";

            english["checkBox9"] = "ib0t support enabled";
            spanish["checkBox9"] = "Soporte de ib0t habilitado";

            english["button2"] = "Open data folder";
            spanish["button2"] = "Abrir la carpeta de datos";

            english["button3"] = "Ares join";
            spanish["button3"] = "Unirse a Ares";

            english["button4"] = "cb0t join";
            spanish["button4"] = "Unirse a cb0t";

            english["column1"] = "Command name";
            spanish["column1"] = "Nombre del comando";

            english["column2"] = "Command level";
            spanish["column2"] = "Nivel del comando";

            english["label5"] = "Owner password:";
            spanish["label5"] = "Contraseña del propietario:";

            english["checkBox10"] = "Enable built in commands";
            spanish["checkBox10"] = "Habilitar construcción en comandos";

            english["checkBox22"] = "Check passwords against clients (strict mode)";
            spanish["checkBox22"] = "Revisar contraseñas a través de Clientes (Modo estricto)";

            english["checkBox24"] = "Unregistered clients can use commands";
            spanish["checkBox24"] = "Clientes sin registro pueden usar los comandos";

            english["label26"] = "Trusted leaves (hub mode)";
            spanish["label26"] = "Hojas de confianza (Modo cubo)";

            english["label27"] = "Trusted leaf identifier:";
            spanish["label27"] = "Identificador hoja de confianza:";

            english["button7"] = "Add";
            spanish["button7"] = "Añadir";

            english["label25"] = "Your leaf identifier:";
            spanish["label25"] = "Su indentificador de hoja:";

            english["label24"] = "Link mode:";
            spanish["label24"] = "Modo link:";

            english["checkBox20"] = "Auto reconnect";
            spanish["checkBox20"] = "Auto reconectar";

            english["checkBox21"] = "Allow linked admin";
            spanish["checkBox21"] = "Permitir linkeado por administador";

            english["label22"] = "Available:";
            spanish["label22"] = "Disponible:";

            english["label23"] = "Refresh...";
            spanish["label23"] = "Actualizar...";

            english["label20"] = "Server avatar:";
            spanish["label20"] = "Avatar del servidor:";

            english["label21"] = "Default avatar:";
            spanish["label21"] = "Avatar por defecto";

            english["button5"] = "Update";
            spanish["button5"] = "Actualizar";

            english["button6"] = "Update";
            spanish["button6"] = "Actualizar";

            english["tab1"] = "Main";
            spanish["tab1"] = "Principal";

            english["tab2"] = "Admin";
            spanish["tab2"] = "Administrador";

            english["tab3"] = "Linking";
            spanish["tab3"] = "Linkeando";

            english["tab4"] = "Advanced";
            spanish["tab4"] = "Avanzado";

            english["tab5"] = "Avatars";
            spanish["tab5"] = "Avatares";

            english["tab6"] = "Extensions";
            spanish["tab6"] = "Extensiones";

            english["label9"] = "Advanced settings";
            spanish["label9"] = "Configuración avanzada";

            english["checkBox7"] = "Enable file browsing";
            spanish["checkBox7"] = "Habilitar navegación de archivos";

            english["checkBox18"] = "Hide IP addresses";
            spanish["checkBox18"] = "Esconder la dirección IP";

            english["checkBox23"] = "Enable room search";
            spanish["checkBox23"] = "Habilitar la búsqueda de salas";

            english["checkBox19"] = "Local clients auto login";
            spanish["checkBox19"] = "Inicio de sesión automática para clientes locales";

            english["label16"] = "ib0t channel list receiver script:";
            spanish["label16"] = "Script receptor de lista de canales de ib0t:";

            english["label17"] = "UDP host address:";
            spanish["label17"] = "Dirección host UDP:";

            english["label18"] = "Preferred language:";
            spanish["label18"] = "Idioma preferido:";

            english["label12"] = "Restrictions";
            spanish["label12"] = "Restricciones";

            english["label14"] = "Minimum age:";
            spanish["label14"] = "Edad mínima:";

            english["checkBox17"] = "Enable";
            spanish["checkBox17"] = "Habilitado";

            english["label15"] = "Gender";
            spanish["label15"] = "Genero";

            english["checkBox15"] = "Reject male";
            spanish["checkBox15"] = "Rechazar hombres";

            english["checkBox14"] = "Reject female";
            spanish["checkBox14"] = "Rechazar mujeres";

            english["checkBox16"] = "Reject unknown";
            spanish["checkBox16"] = "Rechazar desconocidos";

            english["label10"] = "Ban lists";
            spanish["label10"] = "Listas de usuarios no permitidos en la sala";

            english["label11"] = "Interval:";
            spanish["label11"] = "Intervalos:";

            english["label19"] = "hours";
            spanish["label19"] = "Horas";

            english["checkBox13"] = "Auto clear bans";
            spanish["checkBox13"] = "Auto limpieza de bans";

            english["label8"] = "Captcha";
            spanish["label8"] = "Captcha";

            english["label13"] = "Mode:";
            spanish["label13"] = "Modo:";

            english["checkBox8"] = "Captcha enabled";
            spanish["checkBox8"] = "Captcha habilitado";

            english["label7"] = "Scripting";
            spanish["label7"] = "Scripting";

            english["checkBox11"] = "Enable javascript engine";
            spanish["checkBox11"] = "Habilitar motor javascript";

            english["checkBox12"] = "Enable in-room scripting";
            spanish["checkBox12"] = "Habilitar scripting en-sala";

            english["label6"] = "Scripting level:";
            spanish["label6"] = "Nivel para usar\r\nscripting:";

            english["checkBox30"] = "Scripts can change level";
            spanish["checkBox30"] = "Los scripts pueden cambiar de nivel";

            english["checkbox25"] = "Fonts enabled";
            spanish["checkbox25"] = "Fuentes habilitadas";

            IsSpanish = core.Settings.Get<bool>("is_spanish");

            if (IsSpanish)
                SetSpanish(w);
        }

        public static void SetSpanish(MainWindow w)
        {
            core.Settings.Set("is_spanish", true);
            IsSpanish = true;
            w.FontSize = 10;
            w.button1.FontSize = 10;
            w.button2.FontSize = 10;
            w.button3.FontSize = 10;
            w.button4.FontSize = 10;
            w.label6.Height = 48;
            Canvas.SetTop(w.label6, 292);

            if (core.Settings.RUNNING)
            {
                w.button1.Content = spanish["button1b"];
                w.statusLabel.Content = spanish["statuslabelb"];
            }
            else
            {
                w.button1.Content = spanish["button1a"];
                w.statusLabel.Content = spanish["statuslabela"];
            }

            w.label2.Content = spanish["label2"];
            w.label3.Content = spanish["label3"];
            w.label4.Content = spanish["label4"];
            w.checkBox1.Content = spanish["checkBox1"];
            w.checkBox2.Content = spanish["checkBox2"];
            w.checkBox3.Content = spanish["checkBox3"];
            w.checkBox4.Content = spanish["checkBox4"];
            w.checkBox5.Content = spanish["checkBox5"];
            w.checkBox6.Content = spanish["checkBox6"];
            w.checkBox9.Content = spanish["checkBox9"];
            w.button2.Content = spanish["button2"];
            w.button3.Content = spanish["button3"];
            w.button4.Content = spanish["button4"];
            GridView g = (GridView)w.listView1.View;
            g.Columns[0].Header = spanish["column1"];
            g.Columns[1].Header = spanish["column2"];
            w.label5.Content = spanish["label5"];
            w.checkBox10.Content = spanish["checkBox10"];
            w.checkBox22.Content = spanish["checkBox22"];
            w.label26.Content = spanish["label26"];
            w.label27.Content = spanish["label27"];
            w.button7.Content = spanish["button7"];
            w.label25.Content = spanish["label25"];
            w.label24.Content = spanish["label24"];
            w.checkBox20.Content = spanish["checkBox20"];
            w.checkBox21.Content = spanish["checkBox21"];
            w.label22.Content = spanish["label22"];
            w.label23.Content = spanish["label23"];
            w.label20.Content = spanish["label20"];
            w.label21.Content = spanish["label21"];
            w.button5.Content = spanish["button5"];
            w.button6.Content = spanish["button6"];
            w.tab1.Text = spanish["tab1"];
            w.tab2.Text = spanish["tab2"];
            w.tab3.Text = spanish["tab3"];
            w.tab4.Text = spanish["tab4"];
            w.tab5.Text = spanish["tab5"];
            w.tab6.Text = spanish["tab6"];
            w.label9.Content = spanish["label9"];
            w.checkBox7.Content = spanish["checkBox7"];
            w.checkBox18.Content = spanish["checkBox18"];
            w.checkBox23.Content = spanish["checkBox23"];
            w.checkBox19.Content = spanish["checkBox19"];
            w.label16.Content = spanish["label16"];
            w.label17.Content = spanish["label17"];
            w.label18.Content = spanish["label18"];
            w.label12.Content = spanish["label12"];
            w.label14.Content = spanish["label14"];
            w.checkBox17.Content = spanish["checkBox17"];
            w.label15.Content = spanish["label15"];
            w.checkBox15.Content = spanish["checkBox15"];
            w.checkBox14.Content = spanish["checkBox14"];
            w.checkBox16.Content = spanish["checkBox16"];
            w.label10.Content = spanish["label10"];
            w.label11.Content = spanish["label11"];
            w.label19.Content = spanish["label19"];
            w.checkBox13.Content = spanish["checkBox13"];
            w.label8.Content = spanish["label8"];
            w.label13.Content = spanish["label13"];
            w.checkBox8.Content = spanish["checkBox8"];
            w.label7.Content = spanish["label7"];
            w.checkBox11.Content = spanish["checkBox11"];
            w.checkBox12.Content = spanish["checkBox12"];
            w.label6.Content = spanish["label6"];
            w.checkBox30.Content = spanish["checkBox30"];
            w.checkBox25.Content = spanish["checkbox25"];

            w.main.Width = 82;
            w.admin.Width = 103;
            w.linking.Width = 85;
            w.advanced.Width = 83;
            w.avatars.Width = 78;
            w.plugins.Width = 92;
        }

        public static void SetEnglish(MainWindow w)
        {
            core.Settings.Set("is_spanish", false);
            IsSpanish = false;
            w.FontSize = 12;
            w.button1.FontSize = 12;
            w.button2.FontSize = 12;
            w.button3.FontSize = 12;
            w.button4.FontSize = 12;
            w.label6.Height = 28;
            Canvas.SetTop(w.label6, 297);

            if (core.Settings.RUNNING)
            {
                w.button1.Content = english["button1b"];
                w.statusLabel.Content = english["statuslabelb"];
            }
            else
            {
                w.button1.Content = english["button1a"];
                w.statusLabel.Content = english["statuslabela"];
            }

            w.label2.Content = english["label2"];
            w.label3.Content = english["label3"];
            w.label4.Content = english["label4"];
            w.checkBox1.Content = english["checkBox1"];
            w.checkBox2.Content = english["checkBox2"];
            w.checkBox3.Content = english["checkBox3"];
            w.checkBox4.Content = english["checkBox4"];
            w.checkBox5.Content = english["checkBox5"];
            w.checkBox6.Content = english["checkBox6"];
            w.checkBox9.Content = english["checkBox9"];
            w.button2.Content = english["button2"];
            w.button3.Content = english["button3"];
            w.button4.Content = english["button4"];
            GridView g = (GridView)w.listView1.View;
            g.Columns[0].Header = english["column1"];
            g.Columns[1].Header = english["column2"];
            w.label5.Content = english["label5"];
            w.checkBox10.Content = english["checkBox10"];
            w.checkBox22.Content = english["checkBox22"];
            w.label26.Content = english["label26"];
            w.label27.Content = english["label27"];
            w.button7.Content = english["button7"];
            w.label25.Content = english["label25"];
            w.label24.Content = english["label24"];
            w.checkBox20.Content = english["checkBox20"];
            w.checkBox21.Content = english["checkBox21"];
            w.label22.Content = english["label22"];
            w.label23.Content = english["label23"];
            w.label20.Content = english["label20"];
            w.label21.Content = english["label21"];
            w.button5.Content = english["button5"];
            w.button6.Content = english["button6"];
            w.tab1.Text = english["tab1"];
            w.tab2.Text = english["tab2"];
            w.tab3.Text = english["tab3"];
            w.tab4.Text = english["tab4"];
            w.tab5.Text = english["tab5"];
            w.tab6.Text = english["tab6"];
            w.label9.Content = english["label9"];
            w.checkBox7.Content = english["checkBox7"];
            w.checkBox18.Content = english["checkBox18"];
            w.checkBox23.Content = english["checkBox23"];
            w.checkBox19.Content = english["checkBox19"];
            w.label16.Content = english["label16"];
            w.label17.Content = english["label17"];
            w.label18.Content = english["label18"];
            w.label12.Content = english["label12"];
            w.label14.Content = english["label14"];
            w.checkBox17.Content = english["checkBox17"];
            w.label15.Content = english["label15"];
            w.checkBox15.Content = english["checkBox15"];
            w.checkBox14.Content = english["checkBox14"];
            w.checkBox16.Content = english["checkBox16"];
            w.label10.Content = english["label10"];
            w.label11.Content = english["label11"];
            w.label19.Content = english["label19"];
            w.checkBox13.Content = english["checkBox13"];
            w.label8.Content = english["label8"];
            w.label13.Content = english["label13"];
            w.checkBox8.Content = english["checkBox8"];
            w.label7.Content = english["label7"];
            w.checkBox11.Content = english["checkBox11"];
            w.checkBox12.Content = english["checkBox12"];
            w.label6.Content = english["label6"];
            w.checkBox30.Content = english["checkBox30"];
            w.checkBox25.Content = english["checkbox25"];

            w.main.Width = 71;
            w.admin.Width = 75;
            w.linking.Width = 78;
            w.advanced.Width = 92;
            w.avatars.Width = 79;
            w.plugins.Width = 96;
        }
    }
}
