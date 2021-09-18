using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XLua.LuaCallCSharp]
public static class UnityEX
{
    public static void OnClickSet(this Button button,object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        // 防止多次增加
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () =>
            {
                func?.Call();
            });
    }

    public static void OnValueChangedSet(this Slider slider, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(
            (float value) =>
            {
                func?.Call(value);
            });
    }

    public static void OnValueChangedSet(this Toggle toggle, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(
            (bool flag) =>
            {
                func?.Call();
            });
    }
}
