import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import { setWithCredentialsStatus } from "@docspace/common/api/client";
//import history from "../history";

import SettingsStore from "./PortalSettingsStore";
import UserStore from "./UserStore";
import TfaStore from "./TfaStore";
import InfoPanelStore from "./InfoPanelStore";
import {
  logout as logoutDesktop,
  desktopConstants,
} from "@docspace/common/desktop";
import { /*combineUrl,*/ isAdmin } from "@docspace/common/utils";
import {
  /*AppServerConfig,*/ LANGUAGE,
  TenantStatus,
} from "@docspace/common/constants";
//const { proxyURL } = AppServerConfig;

class AuthStore {
  userStore = null;

  settingsStore = null;
  tfaStore = null;
  infoPanelStore = null;

  isLoading = false;
  version = null;

  providers = [];
  isInit = false;

  constructor() {
    this.userStore = new UserStore();

    this.settingsStore = new SettingsStore();
    this.tfaStore = new TfaStore();
    this.infoPanelStore = new InfoPanelStore();

    makeAutoObservable(this);
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    try {
      await this.userStore.init();
    } catch (e) {
      console.error(e);
    }

    const requests = [];
    requests.push(this.settingsStore.init());

    if (this.isAuthenticated) {
      !this.settingsStore.passwordSettings &&
        requests.push(this.settingsStore.getPortalPasswordSettings());
    }

    return Promise.all(requests);
  };
  setLanguage() {
    if (this.userStore.user?.cultureName) {
      localStorage.getItem(LANGUAGE) !== this.userStore.user.cultureName &&
        localStorage.setItem(LANGUAGE, this.userStore.user.cultureName);
    } else {
      localStorage.setItem(LANGUAGE, this.settingsStore.culture || "en-US");
    }
  }
  get isLoaded() {
    let success = false;
    if (this.isAuthenticated) {
      success =
        (this.userStore.isLoaded && this.settingsStore.isLoaded) ||
        this.settingsStore.tenantStatus === TenantStatus.PortalRestore;

      success && this.setLanguage();
    } else {
      success = this.settingsStore.isLoaded;
    }

    return success;
  }

  get language() {
    return (
      (this.userStore.user && this.userStore.user.cultureName) ||
      this.settingsStore.culture ||
      "en"
    );
  }

  get isAdmin() {
    const { user } = this.userStore;
    const { currentProductId } = this.settingsStore;

    if (!user || !user.id) return false;

    return isAdmin(user, currentProductId);
  }

  login = async (user, hash, session = true) => {
    try {
      const response = await api.user.login(user, hash, session);

      if (!response || (!response.token && !response.tfa))
        throw response.error.message;

      if (response.tfa && response.confirmUrl) {
        const url = response.confirmUrl.replace(window.location.origin, "");
        return url;
      }

      setWithCredentialsStatus(true);

      this.reset();

      this.init();

      return this.settingsStore.defaultPage;
    } catch (e) {
      return Promise.reject(e);
    }
  };

  loginWithCode = async (userName, passwordHash, code) => {
    await this.tfaStore.loginWithCode(userName, passwordHash, code);
    setWithCredentialsStatus(true);

    this.reset();

    this.init();

    return Promise.resolve(this.settingsStore.defaultPage);
  };

  thirdPartyLogin = async (SerializedProfile) => {
    try {
      const response = await api.user.thirdPartyLogin(SerializedProfile);

      if (!response || !response.token) throw new Error("Empty API response");

      setWithCredentialsStatus(true);

      this.reset();

      this.init();

      return Promise.resolve(this.settingsStore.defaultPage);
    } catch (e) {
      return Promise.reject(e);
    }
  };

  reset = (skipUser = false) => {
    this.isInit = false;
    this.skipModules = false;
    if (!skipUser) {
      this.userStore = new UserStore();
    }

    this.settingsStore = new SettingsStore();
  };

  logout = async () => {
    await api.user.logout();

    //console.log("Logout response ", response);

    setWithCredentialsStatus(false);

    const { isDesktopClient: isDesktop } = this.settingsStore;

    isDesktop && logoutDesktop();
  };

  get isAuthenticated() {
    return (
      this.userStore.isAuthenticated ||
      this.settingsStore.tenantStatus === TenantStatus.PortalRestore
    );
  }

  getEncryptionAccess = (fileId) => {
    return api.files
      .getEncryptionAccess(fileId)
      .then((keys) => {
        return Promise.resolve(keys);
      })
      .catch((err) => console.error(err));
  };

  replaceFileStream = (fileId, file, encrypted, forcesave) => {
    return api.files.updateFileStream(file, fileId, encrypted, forcesave);
  };

  setEncryptionAccess = (file) => {
    return this.getEncryptionAccess(file.id).then((keys) => {
      return new Promise((resolve, reject) => {
        try {
          window.AscDesktopEditor.cloudCryptoCommand(
            "share",
            {
              cryptoEngineId: desktopConstants.cryptoEngineId,
              file: [file.viewUrl],
              keys: keys,
            },
            (obj) => {
              let resFile = null;
              if (obj.isCrypto) {
                let bytes = obj.bytes;
                let filename = "temp_name";
                resFile = new File([bytes], filename);
              }
              resolve(resFile);
            }
          );
        } catch (e) {
          reject(e);
        }
      });
    });
  };

  setDocumentTitle = (subTitle = null) => {
    let title;

    const currentModule = this.settingsStore.product;
    const organizationName = this.settingsStore.organizationName;

    if (subTitle) {
      if (this.isAuthenticated && currentModule) {
        title = subTitle + " - " + currentModule.title;
      } else {
        title = subTitle + " - " + organizationName;
      }
    } else if (currentModule && organizationName) {
      title = currentModule.title + " - " + organizationName;
    } else {
      title = organizationName;
    }

    document.title = title;
  };

  setProductVersion = (version) => {
    this.version = version;
  };

  setProviders = (providers) => {
    this.providers = providers;
  };

  getOforms = () => {
    const culture =
      this.userStore.user.cultureName || this.settingsStore.culture;

    const promise = new Promise(async (resolve, reject) => {
      let oforms = await api.settings.getOforms(
        `${this.settingsStore.urlOforms}${culture}`
      );

      if (!oforms?.data?.data.length) {
        oforms = await api.settings.getOforms(
          `${this.settingsStore.urlOforms}en`
        );
      }

      resolve(oforms);
    });

    return promise;
  };
}

export default new AuthStore();