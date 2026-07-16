"use client";

import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "Faturalar",
  eyebrow: "Finans yönetimi",
  description: "Tedarikçi faturalarını ve ödeme durumlarını yönetin.",
  endpoint: "/v1/invoices",
  basePath: "/finance/invoices",
  singular: "fatura",
  columns: [
    { key: "invoiceNumber", label: "Fatura No" },
    { key: "totalAmountMinor", label: "Tutar" },
    { key: "currency", label: "Para birimi" },
    { key: "status", label: "Durum", type: "status" },
    { key: "type", label: "Tür" },
    { key: "dueDate", label: "Vade", type: "date" },
  ],
  createFields: [
    { name: "invoiceNumber", label: "Fatura numarası", required: true },
    { name: "totalAmountMinor", label: "Tutar (kuruş)", type: "number", required: true },
    { name: "currency", label: "Para birimi", placeholder: "TRY" },
    { name: "status", label: "Durum" },
    { name: "type", label: "Tür" },
  ],
  editFields: [
    { name: "invoiceNumber", label: "Fatura numarası", required: true },
    { name: "totalAmountMinor", label: "Tutar (kuruş)", type: "number" },
    { name: "currency", label: "Para birimi" },
    { name: "status", label: "Durum" },
    { name: "type", label: "Tür" },
  ],
  detail: {
    title: "Fatura detayı",
    eyebrow: "Finans yönetimi",
    endpoint: (id: string) => `/v1/invoices/${id}`,
    backHref: "/finance/invoices",
    labels: { invoiceNumber: "Fatura No", totalAmountMinor: "Tutar", currency: "Para birimi", status: "Durum", type: "Tür", invoiceDate: "Kesim tarihi", dueDate: "Vade" },
  },
};

export default function InvoicesPage() {
  return <EntityListPage config={config} />;
}
