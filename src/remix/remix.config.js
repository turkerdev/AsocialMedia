/** @type {import('@remix-run/dev').AppConfig} */
module.exports = {
  ignoredRouteFiles: ["**/.*"],
  appDirectory: "src",
  future: {
    v2_errorBoundary: true,
    v2_meta: true,
    v2_routeConvention: true,
    v2_normalizeFormMethod: true,
  },
};
