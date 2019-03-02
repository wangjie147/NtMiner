﻿using NTMiner.Core;
using NTMiner.Hashrate;
using NTMiner.MinerClient;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerClientController : ApiController {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand());
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]RequestBase request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                VirtualRoot.Execute(new CloseNTMinerCommand());
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]StartMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StartMine(request.WorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]StopMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StopMineAsync();
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetMinerName([FromBody]SetMinerNameRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                VirtualRoot.Execute(new SetMinerNameCommand(request.MinerName));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
            if (request == null || string.IsNullOrEmpty(request.PropertyName)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.SetMinerProfileProperty(request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ProfileData GetProfile() {
            try {
                INTMinerRoot root = NTMinerRoot.Current;
                Profile.IMinerProfile minerProfile = root.MinerProfile;
                if (minerProfile == null) {
                    return null;
                }
                ICoin mainCoin;
                if (!root.CoinSet.TryGetCoin(minerProfile.CoinId, out mainCoin)) {
                    return null;
                }
                string dualCoinCode = null;
                string dualCoinPool = null;
                string dualCoinWallet = null;
                bool isDualCoinPoolIsUserMode = false;
                string dualCoinPoolUserName = null;
                double dualCoinWeight = 0;
                bool isAutoDualWeight = true;
                bool isDualCoinEnabled = false;
                string mainCoinPool = null;
                string mainCoinWallet = null;
                bool isMainCoinPoolIsUserMode = false;
                string mainCoinPoolUserName = null;
                Profile.ICoinProfile mainCoinProfile = root.MinerProfile.GetCoinProfile(mainCoin.GetId());
                IPool mainCoinPoolModel;
                if (!root.PoolSet.TryGetPool(mainCoinProfile.PoolId, out mainCoinPoolModel)) {
                    return null;
                }
                mainCoinPool = mainCoinPoolModel.Server;
                mainCoinWallet = mainCoinProfile.Wallet;
                isMainCoinPoolIsUserMode = mainCoinPoolModel.IsUserMode;
                mainCoinPoolUserName = mainCoinPoolModel.UserName;
                Profile.ICoinKernelProfile coinKernelProfile = root.MinerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
                isDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                if (isDualCoinEnabled) {
                    ICoin dualCoin;
                    if (!root.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                        return null;
                    }
                    dualCoinCode = dualCoin.Code;
                    isAutoDualWeight = coinKernelProfile.IsAutoDualWeight;
                    dualCoinWeight = coinKernelProfile.DualCoinWeight;
                    Profile.ICoinProfile dualCoinProfile = root.MinerProfile.GetCoinProfile(coinKernelProfile.DualCoinId);
                    IPool dualCoinPoolModel;
                    if (!root.PoolSet.TryGetPool(dualCoinProfile.PoolId, out dualCoinPoolModel)) {
                        return null;
                    }
                    dualCoinPool = dualCoinPoolModel.Server;
                    dualCoinWallet = dualCoinProfile.Wallet;
                    isDualCoinPoolIsUserMode = dualCoinPoolModel.IsUserMode;
                    dualCoinPoolUserName = dualCoinPoolModel.UserName;
                }
                ProfileData data = new ProfileData {
                    CoinCode = mainCoin.Code,
                    DualCoinCode = dualCoinCode,
                    DualCoinPool = dualCoinPool,
                    DualCoinWallet = dualCoinWallet,
                    IsDualCoinPoolIsUserMode = isDualCoinPoolIsUserMode,
                    DualCoinPoolUserName = dualCoinPoolUserName,
                    DualCoinWeight = dualCoinWeight,
                    IsAutoBoot = minerProfile.IsAutoBoot,
                    IsAutoDualWeight = isAutoDualWeight,
                    IsAutoRestartKernel = minerProfile.IsAutoRestartKernel,
                    IsAutoStart = minerProfile.IsAutoStart,
                    IsDualCoinEnabled = isDualCoinEnabled,
                    IsNoShareRestartKernel = minerProfile.IsNoShareRestartKernel,
                    IsPeriodicRestartComputer = minerProfile.IsPeriodicRestartComputer,
                    IsPeriodicRestartKernel = minerProfile.IsPeriodicRestartKernel,
                    MainCoinPool = mainCoinPool,
                    MainCoinWallet = mainCoinWallet,
                    IsMainCoinPoolIsUserMode = isMainCoinPoolIsUserMode,
                    MainCoinPoolUserName = mainCoinPoolUserName,
                    MinerName = NTMinerRoot.GetMinerName(),
                    NoShareRestartKernelMinutes = minerProfile.NoShareRestartKernelMinutes,
                    PeriodicRestartComputerHours = minerProfile.PeriodicRestartComputerHours,
                    PeriodicRestartKernelHours = minerProfile.PeriodicRestartKernelHours
                };
                return data;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public SpeedData GetSpeed() {
            try {
                SpeedData data = Report.CreateSpeedData();
                return data;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
    }
}
