using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CEError : CustomEvent
{
    public bool isShowWindow = true;//показывать модальное окно ошибки
    public int IDErrorServ; //ID ошибки, которую прислал сервер
    public string Title;//заголовок ошибки
    public string Desc;//описание ошибки
    //имя пакета из апи сервера вызвашую ошибку
    public string ServerResponseEvent;
}
