import type { DashboardResumo } from "./dashboard.types";

export const dashboardMock: DashboardResumo = {
  itens: [
    { pessoaId: 1, nome: "Gabriel", totalReceitas: 5200, totalDespesas: 3100 },
    { pessoaId: 2, nome: "Gabriela", totalReceitas: 4300, totalDespesas: 2500 },
    { pessoaId: 3, nome: "Noah", totalReceitas: 0, totalDespesas: 800 },
  ],
};
