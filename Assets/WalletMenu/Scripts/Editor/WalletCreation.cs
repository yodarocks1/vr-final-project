using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WalletCreation : EditorWindow
{
    [MenuItem("GameObject/WalletMenu/WalletMenu", false, 10)]
    static void CreateWalletMenu(MenuCommand menuCommand)
    {
        GameObject go = new("New Wallet Menu");
        WalletMenu wm = go.AddComponent<WalletMenu>();
        wm.BoundingBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WalletMenu/Prefabs/Internal/BoundingBox.prefab");
        wm.BoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WalletMenu/Prefabs/Internal/Card Box.prefab");
        wm.HoloCardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WalletMenu/Prefabs/Internal/Holocard.prefab");
        wm.TextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WalletMenu/Prefabs/Internal/Text Prefab.prefab");
        wm.CardViewerShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/WalletMenu/Materials and Shaders/PassthroughShader.shader");
        wm.CardDisplayShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/WalletMenu/Materials and Shaders/PassedthroughShader.shader");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, $"Create New Wallet Menu");
        Selection.activeObject = go;
    }

    private static GameObject CreateCard<T>(MenuCommand menuCommand, string name)
        where T : Card
    {
        WalletMenu wm = menuCommand.context as WalletMenu;
        if (wm == null)
        {
            if (!(menuCommand.context as GameObject).TryGetComponent(out wm))
            {
                wm = (menuCommand.context as GameObject).GetComponentInParent<WalletMenu>();
                if (wm == null)
                {
                    throw new ArgumentException("Can only add cards to a WalletMenu");
                }
            }
        }
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.AddComponent<T>();
        DestroyImmediate(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/WalletMenu/Materials and Shaders/Invisible.mat");
        GameObjectUtility.SetParentAndAlign(go, wm.gameObject);
        go.transform.localScale = new Vector3(1, (float)wm.CardHeight, 1);
        go.name = name;
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        Selection.activeObject = go;
        return go;
    }

    [MenuItem("GameObject/WalletMenu/Holo Card", false, 15)]
    [MenuItem("CONTEXT/WalletMenu/New Card/Holo Card")]
    static void CreateHoloCard(MenuCommand menuCommand)
        => CreateCard<HoloCard>(menuCommand, "New Holo Card");

    [MenuItem("GameObject/WalletMenu/Text Card", false, 15)]
    [MenuItem("CONTEXT/WalletMenu/New Card/Text Card")]
    static void CreateTextCard(MenuCommand menuCommand)
    {
        GameObject go = CreateCard<InsetTextCard>(menuCommand, "New Text Card");
        go.GetComponent<InsetTextCard>().Text = "Text Goes Here";
    }

    [MenuItem("GameObject/WalletMenu/Inset Object Card", false, 15)]
    [MenuItem("CONTEXT/WalletMenu/New Card/Inset Object Card")]
    static void CreateInsetObjectCard(MenuCommand menuCommand)
        => CreateCard<InsetObjectCard>(menuCommand, "New Inset Object Card");

    [MenuItem("GameObject/WalletMenu/Object Card", false, 15)]
    [MenuItem("CONTEXT/WalletMenu/New Card/Object Card")]
    static void CreateObjectCard(MenuCommand menuCommand)
        => CreateCard<ObjectCard>(menuCommand, "New Object Card");

    private static void AskForObjectOfType<T>(object context, GenericMenu.MenuFunction2 callback) where T : Component
    {
        List<T> objects = new();
        foreach (GameObject g in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            objects.AddRange(g.GetComponentsInChildren<T>());
        }
        if (objects.Count == 0) throw new ArgumentException($"No {typeof(T).Name}s exist to provide for");
        else if (objects.Count == 1) callback(objects[0]);
        else if (context is GameObject objct && objct.TryGetComponent(out T objT)) callback(objT);
        else if (context is T) callback(context);
        else
        {
            GenericMenu menu = new();
            menu.AddDisabledItem(new GUIContent($"Select a {typeof(T).Name}"));
            foreach (T obj in objects)
            {
                menu.AddItem(new GUIContent(obj.name), false, callback, obj);
            }
            menu.DropDown(new Rect(0, 0, 100, 100));
        }
    }

    [MenuItem("GameObject/XR/WalletMenu/Wallet Provider")]
    [MenuItem("CONTEXT/WalletMenu/Add Provider")]
    static void CreateWalletMenuProvider(MenuCommand menuCommand)
    {
        AskForObjectOfType<WalletMenu>(menuCommand.context, item =>
        {
            Debug.Log(item);
            WalletMenu menu = item as WalletMenu;
            GameObject go = (menuCommand.context is Component c)
                ? c.gameObject
                : new GameObject(menu.name + " Provider");
            XRWalletProvider provider = go.AddComponent<XRWalletProvider>();
            provider.Wallet = menu;
            provider.Viewer = go.transform;
            GameObjectUtility.SetParentAndAlign(go, menu.gameObject);
            Selection.activeObject = go;
        });
    }

    [MenuItem("CONTEXT/XRController/Handle Wallet Selection")]
    static void CreateWalletMenuSelector(MenuCommand menuCommand)
    {
        AskForObjectOfType<XRWalletProvider>(null, item =>
        {
            XRController controller = menuCommand.context as XRController;
            XRWalletProvider provider = item as XRWalletProvider;
            XRWalletSelector selector = controller.gameObject.AddComponent<XRWalletSelector>();
            selector.visibleOrigin = controller.transform;
            selector.WalletProvider = provider;
            Selection.activeObject = controller;
        });
    }

    [MenuItem("CONTEXT/XRController/Handle Wallet Scrolling")]
    static void CreateWalletMenuScroll(MenuCommand menuCommand)
    {
        AskForObjectOfType<XRWalletProvider>(null, item =>
        {
            XRController controller = menuCommand.context as XRController;
            XRWalletProvider provider = item as XRWalletProvider;
            XRWalletScroll scroller = controller.gameObject.AddComponent<XRWalletScroll>();
            scroller.WalletProvider = provider;
            Selection.activeObject = controller;
        });
    }
}
