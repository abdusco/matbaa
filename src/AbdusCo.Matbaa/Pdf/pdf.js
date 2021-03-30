/**
 * @param {any} options.page Puppeteer page
 * @param {Object} options.context PDF generation context
 * @param {string} options.context.html Page content as HTML
 * */
module.exports = async function (options) {
    const { page, context } = options;
    await page.setContent(context.html);
    return { data: await page.pdf(), type: 'application/pdf' };
};