<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.2" tiledversion="1.2.4" name="Main" tilewidth="16" tileheight="16" tilecount="4096" columns="64">
 <image source="TileSheet_Main.png" width="1024" height="1024"/>
 <terraintypes>
  <terrain name="Base ground" tile="70"/>
  <terrain name="Dark ground" tile="65"/>
  <terrain name="Grass A" tile="8"/>
  <terrain name="Stone slabs v2 transition v2" tile="77"/>
  <terrain name="Stone slabs v2 transition v1" tile="82"/>
  <terrain name="Shadow small" tile="386"/>
  <terrain name="Shadow big" tile="389"/>
  <terrain name="Pit" tile="26"/>
  <terrain name="Fire pit" tile="268"/>
  <terrain name="Mud" tile="283"/>
 </terraintypes>
 <tile id="0" type="Ground" terrain="0,0,0,1"/>
 <tile id="1" type="Ground" terrain="0,0,1,1"/>
 <tile id="2" type="Ground" terrain="0,0,1,0"/>
 <tile id="3" type="Ground" terrain="1,1,1,0"/>
 <tile id="4" type="Ground" terrain="1,1,0,1"/>
 <tile id="5" type="Ground" terrain="2,2,2,0"/>
 <tile id="6" type="Ground" terrain="2,2,0,0"/>
 <tile id="7" type="Ground" terrain="2,2,0,2"/>
 <tile id="8" type="Ground" terrain="0,0,0,2"/>
 <tile id="9" type="Ground" terrain="0,0,2,0"/>
 <tile id="10" type="Ground" terrain="2,2,2,2"/>
 <tile id="11" type="Ground" terrain="0,0,0,3"/>
 <tile id="12" type="Ground" terrain="0,0,3,3"/>
 <tile id="13" type="Ground" terrain="0,0,3,0"/>
 <tile id="14" type="Ground" terrain="3,3,3,0"/>
 <tile id="15" type="Ground" terrain="3,3,0,3"/>
 <tile id="16" type="Ground" terrain="0,0,0,4"/>
 <tile id="17" type="Ground" terrain="0,0,4,4"/>
 <tile id="18" type="Ground" terrain="0,0,4,0"/>
 <tile id="19" type="Ground" terrain="4,4,4,0"/>
 <tile id="20" type="Ground" terrain="4,4,0,4"/>
 <tile id="21" type="Pit" terrain="2,2,2,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="22" type="Pit" terrain="2,2,7,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="8" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="23" type="Pit" terrain="2,2,7,2">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="24" type="Pit" terrain="7,7,7,2">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="0">
    <polygon points="0,0 0,16 8,16 8,8 16,8 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="25" type="Pit" terrain="7,7,2,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0">
    <polygon points="0,0 0,8 8,8 8,16 16,16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="26" type="Pit" terrain="3,3,3,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="27" type="Pit" terrain="3,3,7,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="8" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="28" type="Pit" terrain="3,3,7,3">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="29" type="Pit" terrain="7,7,7,3">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="0">
    <polygon points="0,0 0,16 8,16 8,8 16,8 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="30" type="Pit" terrain="7,7,3,7">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="0">
    <polygon points="0,0 0,8 8,8 8,16 16,16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="52" type="RemoveMe"/>
 <tile id="53" type="RemoveMe"/>
 <tile id="54" type="RemoveMe"/>
 <tile id="55" type="RemoveMe"/>
 <tile id="64" type="Ground" terrain="0,1,0,1"/>
 <tile id="65" type="Ground" terrain="1,1,1,1"/>
 <tile id="66" type="Ground" terrain="1,0,1,0"/>
 <tile id="67" type="Ground" terrain="1,0,1,1"/>
 <tile id="68" type="Ground" terrain="0,1,1,1"/>
 <tile id="69" type="Ground" terrain="2,0,2,0"/>
 <tile id="70" type="Ground" terrain="0,0,0,0"/>
 <tile id="71" type="Ground" terrain="0,2,0,2"/>
 <tile id="72" type="Ground" terrain="0,2,0,0"/>
 <tile id="73" type="Ground" terrain="2,0,0,0"/>
 <tile id="75" type="Ground" terrain="0,3,0,3"/>
 <tile id="76" type="Ground" terrain="3,3,3,3"/>
 <tile id="77" type="Ground" terrain="3,0,3,0"/>
 <tile id="78" type="Ground" terrain="3,0,3,3"/>
 <tile id="79" type="Ground" terrain="0,3,3,3"/>
 <tile id="80" type="Ground" terrain="0,4,0,4"/>
 <tile id="81" type="Ground" terrain="4,4,4,4"/>
 <tile id="82" type="Ground" terrain="4,0,4,0"/>
 <tile id="83" type="Ground" terrain="4,0,4,4"/>
 <tile id="84" type="Ground" terrain="0,4,4,4"/>
 <tile id="85" type="Pit" terrain="2,7,2,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="86" type="Pit" terrain="7,7,7,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="87" type="Pit" terrain="7,2,7,2">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="88" type="Pit" terrain="7,2,7,7">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="0">
    <polygon points="0,0 8,0 8,8 16,8 16,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="89" type="Pit" terrain="2,7,7,7">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="8">
    <polygon points="0,0 8,0 8,-8 16,-8 16,8 0,8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="90" type="Pit" terrain="3,7,3,7">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="92" type="Pit" terrain="7,3,7,3">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="93" type="Pit" terrain="7,3,7,7">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="0">
    <polygon points="0,0 8,0 8,8 16,8 16,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="94" type="Pit" terrain="3,7,7,7">
  <objectgroup draworder="index">
   <object id="3" type="Pit" x="0" y="8">
    <polygon points="0,0 8,0 8,-8 16,-8 16,8 0,8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="116" type="RemoveMe"/>
 <tile id="117" type="PlayerBase">
  <properties>
   <property name="X" type="float" value="8"/>
   <property name="Y" type="float" value="-8"/>
  </properties>
 </tile>
 <tile id="118" type="RemoveMe"/>
 <tile id="119" type="RemoveMe"/>
 <tile id="128" type="Ground" terrain="0,1,0,0"/>
 <tile id="129" type="Ground" terrain="1,1,0,0"/>
 <tile id="130" type="Ground" terrain="1,0,0,0"/>
 <tile id="133" type="Ground" terrain="2,0,2,2"/>
 <tile id="134" type="Ground" terrain="0,0,2,2"/>
 <tile id="135" type="Ground" terrain="0,2,2,2"/>
 <tile id="139" type="Ground" terrain="0,3,0,0"/>
 <tile id="140" type="Ground" terrain="3,3,0,0"/>
 <tile id="141" type="Ground" terrain="3,0,0,0"/>
 <tile id="142" type="Ground" terrain="3,3,3,3" probability="0.2"/>
 <tile id="143" type="Ground" terrain="3,3,3,3" probability="0.1"/>
 <tile id="144" type="Ground" terrain="0,4,0,0"/>
 <tile id="145" type="Ground" terrain="4,4,0,0"/>
 <tile id="146" type="Ground" terrain="4,0,0,0"/>
 <tile id="147" type="Ground" terrain="4,4,4,4" probability="0.2"/>
 <tile id="148" type="Ground" terrain="4,4,4,4" probability="0.1"/>
 <tile id="149" type="Pit" terrain="2,7,2,2">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="150" type="Pit" terrain="7,7,2,2">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="151" type="Pit" terrain="7,2,2,2">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="154" type="Pit" terrain="3,7,3,3">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="8" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="155" type="Pit" terrain="7,7,3,3">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="156" type="Pit" terrain="7,3,3,3">
  <objectgroup draworder="index">
   <object id="1" type="Pit" x="0" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="180" type="RemoveMe"/>
 <tile id="181" type="RemoveMe"/>
 <tile id="182" type="RemoveMe"/>
 <tile id="183" type="RemoveMe"/>
 <tile id="192" type="Wall"/>
 <tile id="193" type="Wall"/>
 <tile id="194" type="Wall"/>
 <tile id="195" type="Wall"/>
 <tile id="196" type="Wall"/>
 <tile id="197" type="Wall"/>
 <tile id="198" type="Wall"/>
 <tile id="203" type="FirePit" terrain="0,0,0,8">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="8" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="204" type="FirePit" terrain="0,0,8,8">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="8" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="205" type="FirePit" terrain="0,0,8,0">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="206" type="FirePit" terrain="8,8,8,0">
  <objectgroup draworder="index">
   <object id="3" type="FirePit" x="0" y="0">
    <polygon points="0,0 16,0 16,8 12,8 8,12 8,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="207" type="FirePit" terrain="8,8,0,8">
  <objectgroup draworder="index">
   <object id="3" type="FirePit" x="0" y="0">
    <polygon points="0,0 0,8 8,16 16,16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="218" type="Mud" terrain="0,0,0,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="8" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="219" type="Mud" terrain="0,0,9,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="8" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="220" type="Mud" terrain="0,0,9,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="8" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="221" type="Mud" terrain="9,9,9,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0">
    <polygon points="0,0 0,16 8,16 8,8 16,8 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="222" type="Mud" terrain="9,9,0,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0">
    <polygon points="0,0 0,8 8,8 8,16 16,16 16,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="244" type="RemoveMe"/>
 <tile id="245" type="RemoveMe"/>
 <tile id="246" type="RemoveMe"/>
 <tile id="247" type="RemoveMe"/>
 <tile id="256" type="Wall"/>
 <tile id="260" type="Wall"/>
 <tile id="261" type="Wall"/>
 <tile id="262" type="Wall"/>
 <tile id="263" type="Wall"/>
 <tile id="265" type="Pit"/>
 <tile id="266" type="Pit"/>
 <tile id="267" type="FirePit" terrain="0,8,0,8">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="8" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="268" type="FirePit" terrain="8,8,8,8">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="269" type="FirePit" terrain="8,0,8,0">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="270" type="FirePit" terrain="8,0,8,8">
  <objectgroup draworder="index">
   <object id="3" type="FirePit" x="0" y="0">
    <polygon points="0,0 8,0 8,8 16,8 16,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="271" type="FirePit" terrain="0,8,8,8">
  <objectgroup draworder="index">
   <object id="3" type="FirePit" x="0" y="8">
    <polygon points="0,0 4,0 8,-4 8,-8 16,-8 16,8 0,8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="282" type="Mud" terrain="0,9,0,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="8" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="283" type="Mud" terrain="9,9,9,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="284" type="Mud" terrain="9,0,9,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0" width="8" height="16"/>
  </objectgroup>
 </tile>
 <tile id="285" type="Mud" terrain="9,0,9,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0">
    <polygon points="0,0 8,0 8,8 16,8 16,16 0,16"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="286" type="Mud" terrain="0,9,9,9">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="8">
    <polygon points="0,0 8,0 8,-8 16,-8 16,8 0,8"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="308" type="RemoveMe"/>
 <tile id="309" type="RemoveMe"/>
 <tile id="310" type="RemoveMe"/>
 <tile id="311" type="RemoveMe"/>
 <tile id="320" type="Wall"/>
 <tile id="324" type="Wall"/>
 <tile id="326" type="Wall"/>
 <tile id="327" type="Wall"/>
 <tile id="329" type="Pit"/>
 <tile id="330" type="Pit"/>
 <tile id="331" type="FirePit" terrain="0,8,0,0">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="8" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="332" type="FirePit" terrain="8,8,0,0">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="0" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="333" type="FirePit" terrain="8,0,0,0">
  <objectgroup draworder="index">
   <object id="1" type="FirePit" x="0" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="339" type="Mud" terrain="9,9,9,9" probability="0.1">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0" width="16" height="16"/>
  </objectgroup>
  <animation>
   <frame tileid="340" duration="200"/>
   <frame tileid="344" duration="200"/>
   <frame tileid="345" duration="200"/>
   <frame tileid="349" duration="200"/>
   <frame tileid="350" duration="200"/>
   <frame tileid="339" duration="2000"/>
  </animation>
 </tile>
 <tile id="340" type="Mud"/>
 <tile id="344" type="Mud"/>
 <tile id="345" type="Mud"/>
 <tile id="346" type="Mud" terrain="0,9,0,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="8" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="347" type="Mud" terrain="9,9,0,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0" width="16" height="8"/>
  </objectgroup>
 </tile>
 <tile id="348" type="Mud" terrain="9,0,0,0">
  <objectgroup draworder="index">
   <object id="1" type="Mud" x="0" y="0" width="8" height="8"/>
  </objectgroup>
 </tile>
 <tile id="349" type="Mud"/>
 <tile id="350" type="Mud"/>
 <tile id="372" type="RemoveMe"/>
 <tile id="373" type="PlayerBase">
  <properties>
   <property name="X" type="float" value="8"/>
   <property name="Y" type="float" value="-8"/>
  </properties>
 </tile>
 <tile id="374" type="RemoveMe"/>
 <tile id="375" type="RemoveMe"/>
 <tile id="386" terrain=",,5,"/>
 <tile id="389" terrain=",,6,"/>
 <tile id="393" type="Pit"/>
 <tile id="394" type="Pit"/>
 <tile id="436" type="RemoveMe"/>
 <tile id="437" type="RemoveMe"/>
 <tile id="438" type="RemoveMe"/>
 <tile id="439" type="RemoveMe"/>
 <tile id="450" terrain="5,,5,"/>
 <tile id="452" terrain="6,6,6,6"/>
 <tile id="453" terrain="6,,6,"/>
 <tile id="500" type="RemoveMe"/>
 <tile id="501" type="RemoveMe"/>
 <tile id="502" type="RemoveMe"/>
 <tile id="503" type="RemoveMe"/>
 <tile id="512" terrain=",5,,"/>
 <tile id="513" terrain="5,5,,"/>
 <tile id="514" terrain="5,,,"/>
 <tile id="515" terrain=",6,,"/>
 <tile id="516" terrain="6,6,,"/>
 <tile id="517" terrain="6,,,"/>
 <tile id="526" probability="0.2"/>
 <tile id="527" probability="0.2"/>
 <tile id="4095" type="SpawnPoint"/>
</tileset>
