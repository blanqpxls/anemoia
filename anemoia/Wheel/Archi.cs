using Godot;
using System;
using System.Collections.Generic;

public partial class Archi : Player
{
    public void HandleInventoryAndUI()
    {
        bool interactAreShared = PlayerInput.GrappleAndInteractAreShared;

        if (Main.mapFullscreen)
        {
            Main.mapFullscreen = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.armorWindow)
        {
            Main.armorWindow = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.lootWindow)
        {
            Main.lootWindow = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.playerInventory && !PlayerInput.UsingGamepadUI && !PlayerInput.UsingGamepad)
        {
            Main.playerInventory = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.achievementsWindow)
        {
            Main.achievementsWindow = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.questsWindow)
        {
            Main.questsWindow = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.mapFullscreen && PlayerInput.UsingGamepadUI)
        {
            Main.mapFullscreen = false;
            this.releaseInventory = false;
            SoundEngine.PlaySound(11);
        }
        else if (Main.ingameOptionsWindow)
        {
            if (PlayerInput.UsingGamepadUI && UILinkPointNavigator.CurrentPage == 1002)
                UILinkPointNavigator.ChangePage(1001);
            else
                IngameOptions.Close();
        }
        else if (Main.inFancyUI)
            IngameFancyUI.Close();
        else if (CaptureManager.Instance.Active)
            CaptureManager.Instance.Active = false;
        else if (this.talkNPC >= 0)
        {
            this.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            SoundEngine.PlaySound(11);
            if (PlayerInput.UsingGamepad)
                Main.npcChatRelease = false;
        }
        else if (this.sign >= 0)
        {
            this.sign = -1;
            Main.editSign = false;
            Main.npcChatText = "";
            SoundEngine.PlaySound(11);
        }
        else if (Main.clothesWindow)
            Main.CancelClothesWindow();
        else if (!Main.playerInventory)
        {
            Player.OpenInventory();
        }
        else
        {
            Main.playerInventory = false;
            if (this.channel && Main.mouseItem != null && !Main.mouseItem.IsAir)
                this.channel = false;
            this.tileEntityAnchor.Clear();
            if (!PlayerInput.UsingGamepad)
            {
                Main.EquipPageSelected = 0;
            }
            else
            {
                PlayerInput.NavigatorUnCachePosition();
                Main.GamepadCursorAlpha = 0.0f;
                Player.BlockInteractionWithProjectiles = 3;
                if (PlayerInput.GrappleAndInteractAreShared)
                    this.LockGamepadTileInteractions();
            }
            SoundEngine.PlaySound(11);
            if (ItemSlot.Options.HighlightNewItems)
            {
                foreach (Item obj in this.inventory)
                    obj.newAndShiny = false;
            }
            if (PlayerInput.UsingGamepad)
            {
                Main.npcChatRelease = false;
                this.tileInteractionHappened = true;
                this.releaseInventory = false;
                Main.mouseRight = true;
            }
            else if (PlayerState.Instance != null && PlayerState.Instance.IsInCutscene)
            {
                PlayerState.Instance.SetPlayerControl(false, false, false, false, false, false, false, false);
                PlayerState.Instance.SetPlayerControl(true, true, true, true, true, true, true, true);
            }
            if (this.channel && Main.mouseItem != null && !Main.mouseItem.IsAir)
                this.channel = false;
        }

        if (!interactAreShared)
            return;

        this.GamepadEnableGrappleCooldown();
    }

    private static void OpenInventory()
    {
        Recipe.FindRecipes();
        Main.playerInventory = true;
        Main.EquipPageSelected = 0;
        SoundEngine.PlaySound(10);
    }
}