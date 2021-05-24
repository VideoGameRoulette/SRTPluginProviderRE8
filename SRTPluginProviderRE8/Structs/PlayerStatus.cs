using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE8.Structs.GameStructs
{
    public class PlayerStatus
    {
        public bool IsEthan;
        public bool IsChris;
        public bool IsEnableUpdate;
        public bool IsIdleUpperBody;
        public bool IsGameOver;
        public bool IsBlowDamage;
        public bool IsBlowLand;
        public bool IsHandsGuard;
        public bool IsMeleeGuard;
        public bool IsGunGuard;
        public bool IsExternalGuard;
        public bool IsFrontForbidAttackTarget;
        public bool IsSlipDamage;
        public bool IsNotifyAcidDamage;
        public bool IsInfiniteBulletByFsmAction;
        public bool IsHideShelf;
        public bool IsHollow;
        public bool IsDisableUpperRotate;

        public long BaseAcionID;
        public long UpperActionID;
        public long LArmUpperActionID;
        public long EventActionID;

        public bool IsMeleeAction;
        public bool IsChrisPunch;
        public bool IsGunAttack;
        public bool IsGunAttackLoop;
        public bool IsEnableChangeFovByAim;
        public bool IsSprintForbiddenByOrder;
        public bool IsCrouchForbidden;
        public bool IsForceDisableProgramMovement;
        public bool IsAttackForbiddenByOrder;
        public bool IsGuardForbiddenByOrder;
        public bool IsUpperBodyActionForbiddenByOrder;
        public bool IsInputForbiddenByGUI;
        public bool IsInShop;
        public bool IsInInventoryMenu;
        public bool IsInSelectMenu;
        public bool IsEnableColdBreath;
        public bool IsInSlipArea;
        public bool IsInEscapeSlipArea;
        public bool IsOnBridge;
        public bool IsOnWaterSurface;
        public bool IsExecuteThreeGunMatch;
        public bool IsUseLeftHandWeapon;
        public bool IsUseLeftHandSequence;
        public bool IsDisableUseMine;
        public bool IsFixedAimMode;
        public bool IsEnableUseLaserIrradiation;

        public long PlayerReference;

        public bool IsInWaterAreaCam;
        public bool IsNoReduceBullet;
        public bool IsLoadingNumDouble;
        public bool IsForbidReloadCommand;
        public bool IsForbidAim;

        public PlayerStatus()
        {
        }
        public void Update(GamePlayerStatus gs)
        {
            IsEthan = gs.IsEthan;
            IsChris = gs.IsChris;
            IsEnableUpdate = gs.IsEnableUpdate;
            IsIdleUpperBody = gs.IsIdleUpperBody;
            IsGameOver = gs.IsGameOver;
            IsBlowDamage = gs.IsBlowDamage;
            IsBlowLand = gs.IsBlowLand;
            IsHandsGuard = gs.IsHandsGuard;
            IsMeleeGuard = gs.IsMeleeGuard;
            IsGunGuard = gs.IsGunGuard;
            IsExternalGuard = gs.IsExternalGuard;
            IsFrontForbidAttackTarget = gs.IsFrontForbidAttackTarget;
            IsSlipDamage = gs.IsSlipDamage;
            IsNotifyAcidDamage = gs.IsNotifyAcidDamage;
            IsInfiniteBulletByFsmAction = gs.IsInfiniteBulletByFsmAction;
            IsHideShelf = gs.IsHideShelf;
            IsHollow = gs.IsHollow;
            IsDisableUpperRotate = gs.IsDisableUpperRotate;

            BaseAcionID = gs.BaseAcionID;
            UpperActionID = gs.UpperActionID;
            LArmUpperActionID = gs.LArmUpperActionID;
            EventActionID = gs.EventActionID;

            IsMeleeAction = gs.IsMeleeAction;
            IsChrisPunch = gs.IsChrisPunch;
            IsGunAttack = gs.IsGunAttack;
            IsGunAttackLoop = gs.IsGunAttackLoop;
            IsEnableChangeFovByAim = gs.IsEnableChangeFovByAim;
            IsSprintForbiddenByOrder = gs.IsSprintForbiddenByOrder;
            IsCrouchForbidden = gs.IsCrouchForbidden;
            IsForceDisableProgramMovement = gs.IsForceDisableProgramMovement;
            IsAttackForbiddenByOrder = gs.IsAttackForbiddenByOrder;
            IsGuardForbiddenByOrder = gs.IsGuardForbiddenByOrder;
            IsUpperBodyActionForbiddenByOrder = gs.IsUpperBodyActionForbiddenByOrder;
            IsInputForbiddenByGUI = gs.IsInputForbiddenByGUI;
            IsInShop = gs.IsInShop;
            IsInInventoryMenu = gs.IsInInventoryMenu;
            IsInSelectMenu = gs.IsInSelectMenu;
            IsEnableColdBreath = gs.IsEnableColdBreath;
            IsInSlipArea = gs.IsInSlipArea;
            IsInEscapeSlipArea = gs.IsInEscapeSlipArea;
            IsOnBridge = gs.IsOnBridge;
            IsOnWaterSurface = gs.IsOnWaterSurface;
            IsExecuteThreeGunMatch = gs.IsExecuteThreeGunMatch;
            IsUseLeftHandWeapon = gs.IsUseLeftHandWeapon;
            IsUseLeftHandSequence = gs.IsUseLeftHandSequence;
            IsDisableUseMine = gs.IsDisableUseMine;
            IsFixedAimMode = gs.IsFixedAimMode;
            IsEnableUseLaserIrradiation = gs.IsEnableUseLaserIrradiation;

            PlayerReference = gs.PlayerReference;

            IsInWaterAreaCam = gs.IsInWaterAreaCam;
            IsNoReduceBullet = gs.IsNoReduceBullet;
            IsLoadingNumDouble = gs.IsLoadingNumDouble;
            IsForbidReloadCommand = gs.IsForbidReloadCommand;
            IsForbidAim = gs.IsForbidAim;
        }

    }
}