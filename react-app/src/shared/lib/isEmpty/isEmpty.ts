/**
 * Check if value is empty
 *
 * @param {mixed} mixedVar
 */
function isEmpty(mixedVar: unknown) {
  let key
  if (
    mixedVar === '' ||
    mixedVar === 0 ||
    mixedVar === null ||
    mixedVar === false ||
    mixedVar === undefined
  ) {
    return true
  }
  if (typeof mixedVar == 'object') {
    for (key in mixedVar) {
      return false
    }
    return true
  }
  return false
}

export default isEmpty
