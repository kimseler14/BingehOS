"use client";

import { createContext, useContext, useEffect, useMemo, useState } from "react";
import type { LoginResponse } from "../lib/types";

type AuthContextValue = {
  user: LoginResponse | null;
  ready: boolean;
  setSession: (session: LoginResponse) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<LoginResponse | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    queueMicrotask(() => {
      const raw = window.localStorage.getItem("bingehos.user");
      if (raw) {
        try {
          setUser(JSON.parse(raw) as LoginResponse);
        } catch {
          window.localStorage.removeItem("bingehos.user");
        }
      }
      setReady(true);
    });
  }, []);

  const value = useMemo(
    () => ({
      user,
      ready,
      setSession: (session: LoginResponse) => {
        window.localStorage.setItem("bingehos.accessToken", session.accessToken);
        window.localStorage.setItem("bingehos.user", JSON.stringify(session));
        setUser(session);
      },
      logout: () => {
        window.localStorage.removeItem("bingehos.accessToken");
        window.localStorage.removeItem("bingehos.user");
        setUser(null);
        window.location.assign("/login");
      },
    }),
    [ready, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth AuthProvider içinde kullanılmalıdır.");
  return context;
}
