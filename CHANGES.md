# 1.1.1

  * New feature: Feedback sheet generation. Feedback sheets for every
    drawn round can now be generated by selecting the round in the GUI,
    then pressing the new 'Feedback' button. A new TeX template,
    'feedback-tmpl.tex', is provided as a starting point. Automatic feedback
    sheet generation aids in avoiding confusion with incorrectly or
    incompletely filled in forms by filling in all the important
    organisational information beforehand.
  * Fixes a bug where the correct Mono runtime autodetection would fail to
    choose the correct runtime version.
  * Minor documentation improvements.

# 1.1.0

  * Fractional point averages: Imprecise numbers -- specifically speaker and
    team points averaged over multiple judges -- are now computed with an
    accuracy of 2 rather than 0 decimal places, as mandated by new directives
    from the OPD rules commission.