import React from "react";
import { inject, observer } from "mobx-react";
import { useLocation } from "react-router-dom";

import { SettingsSectionBodyContent } from "../Section";

import Loaders from "@docspace/common/components/Loaders";

const SettingsView = ({
  isLoading,
  isLoadedSettingsTree,

  isAdmin,
}) => {
  const location = useLocation();

  const inLoad = (!isLoadedSettingsTree && isLoading) || isLoading;

  const setting = location.pathname.includes("/settings/common")
    ? "common"
    : "admin";
  return (
    <>
      {inLoad ? (
        setting === "common" ? (
          <Loaders.SettingsCommon isAdmin={isAdmin} />
        ) : (
          <Loaders.SettingsAdmin />
        )
      ) : (
        <SettingsSectionBodyContent />
      )}
    </>
  );
};

export default inject(({ auth, filesStore, settingsStore }) => {
  const { isLoading } = filesStore;

  const { isLoadedSettingsTree } = settingsStore;

  return {
    isLoading,
    isLoadedSettingsTree,

    isAdmin: auth.isAdmin,
  };
})(observer(SettingsView));